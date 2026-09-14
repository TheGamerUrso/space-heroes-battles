using System;
using System.Collections;
using UnityEngine;

public class PlayerShip : Ship, IDamagable
{
    public Action<int> OnItemPickedUp;
    [Space()]
    private PlayerController shipController;
    [SerializeField] private ParticleSystem ItemCollectedEffect;
    [Space()]
    [SerializeField] private Player_SO playerStats;

    private PlayerData playerData;
    private PlayerShipData playerShipData;

    [Space()]
    #region Weapons
    [Header("Weapons")]
    [SerializeField] private DefaultPlayerWeapon[] Weapons;
    [SerializeField] private BaseSpecialAttack specialAttack;
    private float invisibilityTimer;
    private int clicktimes;
    private float clicktimer;
    private bool clicked;
    float clickDelay = .25f;
    #endregion Weapons

    private bool TempFireRateUpgrade;
    private bool HasArmorUprade;
    private IDataService dataService;

 
    //=================================================================================
    public override void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        shipController = GetComponent<PlayerController>();

        playerData = dataService.GetPlayerData();
    
    }
    //=================================================================================
    public override void Start()
    {       
        playerData.NewGame();
        playerShipData = playerData.GetCurrentPlayerShipData();

        shipController.SetSpeed(playerShipData.Speed);

        playerShipData.NewGame(ref HasShield);
        ShieldEffect.SetActive(HasShield);

        SetStats(playerShipData.level);

        SwitchWeapon(0);
        specialAttack.SetOwner(this);

        HealthBar = GameObject.FindAnyObjectByType<PlayerHealthWidget>();
        HealthBar.Setup(this);

        IsAlive = true;
    }

    //=================================================================================
    public void OnLevelValueChanged(int Level)
    {
        SetStats(Level);
    }
    //=================================================================================
    public override void Update()
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
    //=================================================================================
    public void WeaponSystem()
    {
        if (GetAnimationState("Enter") || GetAnimationState("Exit"))
        {
            return;
        }

        var playerPowerUp = playerData.GetPowerUpLevelPresentage();
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
            clicktimes = touch.tapCount;
        }
#elif UNITY_STANDALONE || UNITY_WEBGL || UNITY_EDITOR_64
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire3"))
            {
                if (!clicked)
                {
                    clicked = true;
                    clicktimer = clickDelay;
                }
                clicktimes++;
            }


            if (clicked)
            {
                clicktimer -= Time.deltaTime;

                if (clicktimer <= 0)
                {
                    clicked = false;
                    clicktimes = 0;
                }
            }
#endif

        if (clicktimes > 1)
        {
            if (playerPowerUp >= 1)
            {
                ActivateSpecial();
            }
        }

        if (playerData.PowerPackCollected >= 5)
        {
            playerData.PowerPackCollected = 0;
            UpgradeWeapon();
        }
    }
    //=================================================================================
    public void UpdateWeaponStats(float fireRate, float damage = 0)
    {
        var currenActivetWeapon = GetCurrentActiveWeapon().GetComponent<BaseWeapon>();
        currenActivetWeapon.FireRate = fireRate;
        if (damage > 0)
            currenActivetWeapon.Damage = damage;
    }
    //=================================================================================
    public override void InstallShield()
    {
        base.InstallShield();
        ShieldEffect.SetActive(HasShield);
        OnItemPickedUp?.Invoke(0);
    }
    //=================================================================================
    public override void Death()
    {
        GameObject explostion = PoolManager.Instance.GetObjectFromPool(playerStats.ExplostionEffect);
        explostion.transform.position = transform.position;
        explostion.SetActive(true);

        gameObject.SetActive(false);
    }
    //=================================================================================
    public override void Heal(float ammount)
    {
        CurrentHealth += ammount;

        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }

        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }
    //=================================================================================
    public override void TakeDamage(float dmg)
    {
        if (!IsAlive) return;

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

                if (!HasArmorUprade)
                {
                    DownGradeWeapon();
                }
                playerData.GotHitInGame = true;

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
    //=================================================================================
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
    //=================================================================================
    public void tempGodMode()
    {
        invisibilityTimer = 1;
    }
    //=================================================================================

    public bool GetAnimationState(string id)
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        return animator.GetCurrentAnimatorStateInfo(0).IsName(id);
    }
    //=================================================================================
    public override void OnEnable()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        animator.SetTrigger(Constants.PLAYERENTERSTRINGKEY);
    }
    //=================================================================================
    public override void ExitLevel()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        animator.SetTrigger(Constants.PLAYEREXITSTRINGKEY);
    }
    //=================================================================================
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
    //=================================================================================

    public void GiveTemporaryFireRateBuff()
    {
        StartCoroutine(TemporaryFireRateUpgrade());
    }
    //=================================================================================
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

    //=================================================================================
    public void UpgradeWeapon()
    {
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
                playerData.SetSuperMeter(playerData.PowerUpLevel + 0.025f);
            }

            TempFireRateUpgrade = false;
            SwitchWeapon(CurrentWeapnType);
        }
    }
    //=================================================================================
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
    //=================================================================================
    public void PowerUpCollected()
    {
        if (playerData.PowerPackCollected <= 5 && CurrentWeapnType < 4)
        {
            playerData.SetPowerPackCollected(2);
            TempFireRateBuff(0.01f * playerData.PowerPackCollected);
        }
             OnItemPickedUp?.Invoke(1);
    }
    //=================================================================================
    public void ResetWeaponUpgrade()
    {
        CurrentWeapnType = 0;
        SwitchWeapon(CurrentWeapnType);
    }
    //=================================================================================
    public void ActivateSpecial()
    {
        playerData.SuperUsed++;
        specialAttack.ActivateSpecial();
    }
    //=================================================================================
    public void DeactivateSpecial()
    {
        specialAttack.DeactivateSpecial();
    }
    //=================================================================================
    public GameObject GetCurrentActiveWeapon()
    {
        return Weapons[CurrentWeapnType].gameObject;
    }
    //=================================================================================
    public override void SwitchWeapon(int Id, bool Solo = false)
    {
        for (int i = 0; i < Weapons.Length; i++)
        {
            Weapons[i].gameObject.SetActive(false);
        }

        Weapons[Id].gameObject.SetActive(true);
        Weapons[Id].SetStats(playerShipData, Id);
    }
    //=================================================================================
    public BaseSpecialAttack GetSpecialAttack()
    {
        return specialAttack;
    }
    //=================================================================================
    public override void SetStats(int level)
    {
        MaxHealth = playerShipData.level * playerStats.baseHealth;
        SetHealth(MaxHealth);

        float[] UpgradeStats = playerShipData.GetCalculatedUpgradeStats();
        playerShipData.Speed = playerStats.baseSpeed + UpgradeStats[(int)UpgradeTypeEnum.Speed];
        playerShipData.Damage = (level * playerStats.baseDamage) + UpgradeStats[(int)UpgradeTypeEnum.Damage];
        playerShipData.FireRate = playerStats.baseFireRate - UpgradeStats[(int)UpgradeTypeEnum.FireRate];
        playerShipData.SuperDamage = (level * playerStats.baseSuperDamage) + UpgradeStats[(int)UpgradeTypeEnum.SuperDamage];
        playerShipData.SuperChargeTime = playerStats.baseSpecialCountdown - UpgradeStats[(int)UpgradeTypeEnum.SuperrechargeTime];
        playerShipData.MagnetPower = UpgradeStats[(int)UpgradeTypeEnum.MagnetStrength];
        playerShipData.MagnetDistance = UpgradeStats[(int)UpgradeTypeEnum.MagnetDistance];
        HasArmorUprade = UpgradeStats[(int)UpgradeTypeEnum.ArmorUpgrade] == 1 ? true : false;


        for (int i = 0; i < Weapons.Length; i++)
        {
            Weapons[i].SetStats(playerShipData, CurrentWeapnType);
        }

        specialAttack.SetStats(playerShipData);
    }
    //=================================================================================
    public void SetWallet(int coin)
    {
        playerData.AddCoin(coin);
        GuiManager.CreateFloatingText("<color=" + "yellow" + "> $ </color>", transform.localPosition);
        {
            PlayerPrefs.SetInt("CoinTut", 1);
        }
    }
    //=================================================================================
    public override void EnterLevel()
    {

    }
}
