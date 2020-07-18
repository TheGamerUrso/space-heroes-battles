using System.Collections;
using UnityEngine;

public class PlayerShip : Ship
{
    public Player_SO playerStats;
    private PlayerData playerData;
    private PlayerShipData playerShipData;

    [SerializeField] private PlayerWeapon[] Weapons;
    [SerializeField] private SpecialAttack specialAttack;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private ParticleSystem ItemCollectedEffect;



    #region Weapons
    private bool TempFireRateUpgrade;
    private float invisibilityTimer;
    private int clicktimes;
    private float clicktimer;
    private bool clicked;

    public int CurrentWeapnType { get; private set; } = 0;

    #endregion Weapons

    public override void OnDestroy()
    {
        Events.OnLevelValueChanged -= OnLevelValueChanged;
    }

    public override void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public override void Start()
    {
        IsAlive = true;

        playerData = PersistantData.GetPlayerData();

        for (int i = 0; i < Weapons.Length; i++)
        {
            if (Weapons[i].gameObject.activeSelf)
            {
                Weapons[i].SetShipTransform(transform);
            }
        }

        specialAttack.SetShipTransform(transform);

        Events.PlayerShipHit += DownGradeWeapon;

        SwitchWeapon(0);

        playerShipData = playerData.GetCurrentPlayerShipData();

        HasShield = playerShipData.HasShield;

        ShieldEffect.SetActive(HasShield);

        SetStats(playerShipData.level);

        playerData.powerUpLevel = 0;
        playerData.powerPackCollected = 0;

        Events.OnLevelValueChanged += OnLevelValueChanged;
    }

    public void OnLevelValueChanged(int Level)
    {
        SetStats(Level);
    }

    public override void Update()
    {
        if (GameController.CurrentGameState == GameController.GameState.GAME)
        {
            if (Time.frameCount % 1 == 0)
            {
                if (invisibilityTimer >= 0)
                {
                    invisibilityTimer -= Time.deltaTime;
                }
                WeaponSystem();
            }
        }
    }

    public void WeaponSystem()
    {
        if (GetAnimationState("Enter") || GetAnimationState("Exit"))
        {
            return;
        }

        float playerPowerUp = 0;

        playerPowerUp = playerData.GetPowerUpLevelPresentage();


#if UNITY_ANDROID

        if (Time.timeScale == 0)
        {
            clicked = false;
            clicktimer = 1;
            clicktimes = 0;
            return;
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (playerPowerUp >= 1)
            {
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        clicked = true;
                        clicktimes++;
                        break;
                    case TouchPhase.Moved:
                        break;

                    case TouchPhase.Stationary:
                        break;

                    case TouchPhase.Ended:
                        break;

                    case TouchPhase.Canceled:
                        break;

                    default:
                        break;
                }

                if (clicked && clicktimer > 0)
                {
                    clicktimer -= Time.deltaTime;
                    if (clicktimer <= 0)
                    {
                        clicked = false;
                        clicktimer = 1;
                        clicktimes = 0;
                    }
                }

                if (clicktimes > 1 || touch.tapCount > 2)
                {
                    clicktimes = 0;
                    ActivateSpecial();
                }
            }
        }
#endif

        if (playerData.powerPackCollected >= 5)
        {
            playerData.powerPackCollected = 0;
            UpgradeWeapon();
        }
    }



    public void UpdateWeaponStats(float fireRate, float damage = 0)
    {
        WeaponScript weapon = GetCurrentActiveWeapon().GetComponent<WeaponScript>();

        weapon.SetFireRate(fireRate);
        if (damage > 0)
            weapon.SetDamage(damage);

    }

    public override void InstallShieldModule()
    {
        base.InstallShieldModule();
        ShieldEffect.SetActive(HasShield);
    }

    public override void Death()
    {
        GameObject explostion = PoolManager.Instance.GetObjectFromPool(playerStats.ExplostionEffect);
        explostion.transform.position = transform.position;

        Events.PlayerLost?.Invoke();
        gameObject.SetActive(false);
    }

    public override void Heal(float ammount)
    {
        base.Heal(ammount);
    }

    public override void TakeDamage(float dmg)
    {
        if (IsAlive == false)
        {
            return;
        }

        audioSource.PlayOneShot(playerStats.hitSFX);

        if (dmg >= MaxHealth)
        {
            dmg = MaxHealth - 1;
        }

        if (HasShield == true)
        {
            HasShield = false;
            ShieldEffect.SetActive(HasShield);
        }
        else if (HasShield == false)
        {
            if (invisibilityTimer <= 0)
            {
                invisibilityTimer = .25f;
                CurrentHealth = CurrentHealth - dmg;

                GameSession.Multiplier = 1;

                Events.PlayerShipHit?.Invoke();

                if (playerData.GotHitInGame == false)
                {
                    PlayerData playerData = PersistantData.GetPlayerData();
                    ObjectiveData objectiveData = playerData.GetOnGoingObjectiveById(ObjectiveType.Unharmed);
                    if (objectiveData != null)
                        objectiveData.UpdateProgress(0);
                    playerData.GotHitInGame = true;
                }

                if (GetHealthPresentage() < .5f)
                {
                    audioSource.PlayOneShot(playerStats.alarmSFX);
                }

                if (CurrentHealth < 1)
                {
                    Death();
                }
            }
        }
    }

    public override void OnTriggerEnter(Collider other)
    {
        IPickable items = other.GetComponent<IPickable>();
        if (items != null)
        {
            items.Action(this);
            if (items.ID.Equals("PowerUP"))
                ItemCollectedEffect.Play();
        }
    }

    public void tempGodMode()
    {
        invisibilityTimer = 1;
    }


    public bool GetAnimationState(string id)
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        return animator.GetCurrentAnimatorStateInfo(0).IsName(id);
    }

    public override void OnEnable()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        animator.SetTrigger(Constants.PLAYERENTERSTRINGKEY);
    }

    public void Exit()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        animator.SetTrigger(Constants.PLAYEREXITSTRINGKEY);
    }

    #region TempfireRateBuff

    public void TempFireRateBuff(float fireRate = 0.0f, bool temporary = false)
    {

        if (!TempFireRateUpgrade)
        {
            TempFireRateUpgrade = true;
            GiveTemporaryFireRateBuff();
        }


        UpdateWeaponStats(FireRate - fireRate);
    }

    public void GiveTemporaryFireRateBuff()
    {
        StartCoroutine(TemporaryFireRateUpgrade());
    }

    public IEnumerator TemporaryFireRateUpgrade()
    {
        var fireRateTemp = FireRate;
        var DamageTemp = Damage;

        while (TempFireRateUpgrade)
        {
            yield return new WaitForEndOfFrame();
        }

        FireRate = fireRateTemp;
        UpdateWeaponStats(FireRate, DamageTemp);
    }

    #endregion


    public void UpgradeWeapon()
    {
        if (!PlayerPrefs.HasKey("UpgradeTut"))
        {
            Tutorial.Instance.ShowTutorial(5);
            PlayerPrefs.SetInt("UpgradeTut", 1);
        }

        if (CurrentWeapnType < 4)
        {
            if (playerStats.CanUsePowerUpItem)
            {
                audioSource.PlayOneShot(playerStats.powerSFX);

                CurrentWeapnType++;

                if (CurrentWeapnType > 4)
                {
                    CurrentWeapnType = 4;
                }
            }
            else
            {
                if (playerData != null)
                {
                    playerData.IncreasePowerUp(.025f);
                }

            }

            TempFireRateUpgrade = false;
            SwitchWeapon(CurrentWeapnType);
        }
    }

    public void DownGradeWeapon()
    {
        if (playerShipData.HasArmorUpgrade)
        {
            return;
        }

        if (CurrentWeapnType > 0)
        {
            CurrentWeapnType--;
            SwitchWeapon(CurrentWeapnType);
        }

    }

    public void PowerUpCollected()
    {
        if (playerData.powerPackCollected <= 5 && CurrentWeapnType < 4)
        {
            playerData.PowerPackCollected += 2;
            TempFireRateBuff(0.01f * playerData.powerPackCollected);
        }
    }

    public void ResetWeaponUpgrade()
    {
        CurrentWeapnType = 0;
        SwitchWeapon(CurrentWeapnType);
    }

    public void ActivateSpecial()
    {
        specialAttack.ActivateSpecial();
    }

    public void DeactivateSpecial()
    {
        specialAttack.DeactivateSpecial();
    }

    public GameObject GetCurrentActiveWeapon()
    {
        return Weapons[CurrentWeapnType].gameObject;
    }

    public void SwitchWeapon(int WeaponTypeIndex)
    {
        CurrentWeapnType = WeaponTypeIndex;

        for (int i = 0; i < Weapons.Length; i++)
        {
            Weapons[i].gameObject.SetActive(false);
        }

        Weapons[WeaponTypeIndex].gameObject.SetActive(true);
    }

    public SpecialAttack GetSpecialAttack()
    {
        return specialAttack;
    }

    public override void SetStats(int level)
    {
        base.SetStats(level);
        //Speed 0 
        //FireRate 1
        //Damage 2
        //SuperDamage 3
        //SuperCooldown 4
        //MagnetStrength 5 
        //MagnetDistance 6
        //Shield
        //Armor

        float[] UpgradeStats = playerShipData.GetCalculatedUpgradeStats();

        Speed += UpgradeStats[(int)UpgradeTypeEnum.Speed];
        playerShipData.Speed = Speed;
        //Debug.Log("Speed:" + Speed);
        Damage += UpgradeStats[(int)UpgradeTypeEnum.Damage];
        //Debug.Log("Damage: " + Damage);
        FireRate -= UpgradeStats[(int)UpgradeTypeEnum.FireRate];
        //Debug.Log("FireRate: " + FireRate);
        playerShipData.SuperDamage = (level * playerStats.baseSuperDamage) + UpgradeStats[(int)UpgradeTypeEnum.SuperDamage];
        //Debug.Log("SuperDamage: " + playerShipData.SuperDamage);
        playerShipData.SuperChargeTime = playerStats.baseSpecialCountdown - UpgradeStats[(int)UpgradeTypeEnum.SuperrechargeTime];
        //Debug.Log("SuperChargeTime: " + playerShipData.SuperChargeTime);
        playerShipData.MagnetPower = UpgradeStats[(int)UpgradeTypeEnum.MagnetStrength];
        //Debug.Log("MagnetPower: " + playerShipData.MagnetPower);
        playerShipData.MagnetDistance = UpgradeStats[(int)UpgradeTypeEnum.MagnetDistance];
        //Debug.Log("MagnetDistance: " + playerShipData.MagnetDistance);

    }
}
