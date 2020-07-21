using System;
using System.Collections;
using UnityEngine;

public class PlayerShip : Ship, IDamagable
{
    public event Action<float, float> OnHealthChanged;

    [SerializeField] private Player_SO playerStats;
    [SerializeField] private PlayerWeapon[] Weapons;
    [SerializeField] private SpecialAttack specialAttack;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private ParticleSystem ItemCollectedEffect;

    private WeaponScript weapon;
    private PlayerData playerData;
    private PlayerShipData playerShipData;


    #region Weapons
    private bool TempFireRateUpgrade;
    private float invisibilityTimer;
    private int clicktimes;
    private float clicktimer;
    private bool clicked;

    public bool CanFire { get; private set; }
    public float MaxHealth { get; private set; }
    public float CurrentHealth { get; private set; }
    public int CurrentWeapnType { get; set; }


    private bool HasArmorUprade;

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
        playerData = PersistantData.GetPlayerData();

        if (!HasArmorUprade)
        {
            Events.PlayerShipHit += DownGradeWeapon;
        }

        SwitchWeapon(0);

        playerShipData = playerData.GetCurrentPlayerShipData();

        HasShield = playerShipData.HasShield;

        ShieldEffect.SetActive(HasShield);

        SetStats(playerShipData.level);


        playerData.NewGame();

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

        if (playerData.PowerPackCollected >= 5)
        {
            playerData.PowerPackCollected = 0;
            UpgradeWeapon();
        }
    }



    public void UpdateWeaponStats(float fireRate, float damage = 0)
    {
        weapon = GetCurrentActiveWeapon().GetComponent<WeaponScript>();

        weapon.weaponData.FireRate = fireRate;
        if (damage > 0)
            weapon.weaponData.Damage = damage;

    }

    public override void InstallShieldModule()
    {
        base.InstallShieldModule();
        ShieldEffect.SetActive(HasShield);

        GuiManager.CreateFloatingText("Shield Up", transform.localPosition);

        if (!PlayerPrefs.HasKey("ShieldTut"))
        {
            if (Tutorial.Instance)
            {
                Tutorial.Instance.ShowTutorial(3);
            }
            PlayerPrefs.SetInt("ShieldTut", 1);
        }
    }

    public void Death()
    {
        Game.UseSlowMo = false;
        GameObject explostion = PoolManager.Instance.GetObjectFromPool(playerStats.ExplostionEffect);
        explostion.transform.position = transform.position;

        Events.PlayerLost?.Invoke();
        gameObject.SetActive(false);
    }

    public void Heal(float ammount)
    {
        CurrentHealth += ammount;

        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }

        GuiManager.CreateFloatingText("Heal up", transform.localPosition);

        if (!PlayerPrefs.HasKey("HealTut"))
        {
            if (Tutorial.Instance)
            {
                Tutorial.Instance.ShowTutorial(1);
            }
            PlayerPrefs.SetInt("HealTut", 1);
        }
    }

    public void TakeDamage(float dmg)
    {
        if (IsAlive())
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
                var health = CurrentHealth - dmg;
                SetHealth(health);
                Game.Multiplier = 1;

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

    public void OnTriggerEnter(Collider other)
    {
        IPickable items = other.GetComponent<IPickable>();
        if (items != null)
        {
            items.PickUp();
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


        UpdateWeaponStats(playerShipData.FireRate - fireRate);
    }

    public void GiveTemporaryFireRateBuff()
    {
        StartCoroutine(TemporaryFireRateUpgrade());
    }

    public IEnumerator TemporaryFireRateUpgrade()
    {
        var fireRateTemp = playerShipData.FireRate;
        var DamageTemp = playerShipData.Damage;

        while (TempFireRateUpgrade)
        {
            yield return new WaitForEndOfFrame();
        }

        playerShipData.FireRate = fireRateTemp;
        UpdateWeaponStats(playerShipData.FireRate, DamageTemp);
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
        if (playerData.PowerPackCollected <= 5 && CurrentWeapnType < 4)
        {
            playerData.SetPowerPackCollected(2);
            TempFireRateBuff(0.01f * playerData.PowerPackCollected);
        }

        GuiManager.CreateFloatingText("Power Up", transform.localPosition);

        if (!PlayerPrefs.HasKey("PowerTut"))
        {
            if (Tutorial.Instance)
            {
                Tutorial.Instance.ShowTutorial(2);
            }
            PlayerPrefs.SetInt("PowerTut", 1);
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
        MaxHealth = playerShipData.level * playerStats.baseHealth;
        SetHealth(MaxHealth);

        float[] UpgradeStats = playerShipData.GetCalculatedUpgradeStats();
        playerShipData.Speed = playerStats.baseSpeed + UpgradeStats[(int)UpgradeTypeEnum.Speed];
        playerShipData.Damage = playerStats.baseDamage + UpgradeStats[(int)UpgradeTypeEnum.Damage];
        playerShipData.FireRate = playerStats.baseFireRate - UpgradeStats[(int)UpgradeTypeEnum.FireRate];
        playerShipData.SuperDamage = (level * playerStats.baseSuperDamage) + UpgradeStats[(int)UpgradeTypeEnum.SuperDamage];
        playerShipData.SuperChargeTime = playerStats.baseSpecialCountdown - UpgradeStats[(int)UpgradeTypeEnum.SuperrechargeTime];
        playerShipData.MagnetPower = UpgradeStats[(int)UpgradeTypeEnum.MagnetStrength];
        playerShipData.MagnetDistance = UpgradeStats[(int)UpgradeTypeEnum.MagnetDistance];
        HasArmorUprade = UpgradeStats[(int)UpgradeTypeEnum.ArmorUpgrade] == 1 ? true : false;
    }

    public void SetHealth(float health)
    {
        CurrentHealth = health;
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }

    public float GetHealthPresentage()
    {
        return (CurrentHealth / MaxHealth) * 100;
    }


    public bool IsAlive()
    {
        return CurrentHealth <= 0;
    }

    public void SetWallet(int coin)
    {
        Game.CoinPicked += coin;
        GuiManager.CreateFloatingText("$", transform.localPosition);

        if (!PlayerPrefs.HasKey("CoinTut"))
        {
            if (Tutorial.Instance)
            {
                Tutorial.Instance.ShowTutorial(0);
            }

            PlayerPrefs.SetInt("CoinTut", 1);
        }
    }


    public void EnableFire()
    {
        CanFire = true;
    }
    public void DisableFire()
    {
        CanFire = false;
    }
}
