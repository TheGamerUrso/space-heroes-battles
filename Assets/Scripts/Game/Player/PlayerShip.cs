using System;
using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.PoolSystem;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerShip : Ship, IDestroyable
{
    public Action<float> PowerUpLevelChanged;
    public Action PlayerShipHit;
    public Action PlayerShipDeath;
    public Action<ItemData> PickUpItem;

    public int playerID;

    private PlayerData playerData;

    [Header("Player Config")]
    private SimpleShipControls shipController;

    [Header("Extra Effects")]
    public ParticleSystem ItemCollectedEffect;

    /**
     * Statistics
    */
    private float invisibilityTimer;
    private bool GotHit;
    private int SuperUsed;
    public bool CanUsePowerUpItem;
    public static bool TempFireRateUpgrade { get; set; }
    public bool IsPlayerDamaged
    {
        get
        {
            return GotHit;
        }
    }

    /**
     *Upgrades
     */
    #region Upgrades

    private float GameControllerSpeedValue = 0;
    private float GameControllerDamageValue = 0;
    private float GameControllerFireRateValue = 0;
    private float GameControllerMagnetPowerValue = 0;
    private float GameControllerActivtateDistanceValue = 0;
    private float GameControllerSuperTime = 0;
    private float GameControllerSuperDamage = 0;

    [SerializeField] private float[] Upgrades;

    public bool armorUpgrade;
    public bool HasArmorUpgrade
    {
        get
        {
            return armorUpgrade;
        }

        set
        {
            armorUpgrade = value;
        }
    }
    #endregion

    /**
    * Attributes
    */
    #region Attributes
    [Min(0)]
    public float SuperDamage;
    [Min(0)]
    public float SuperChargeTime;
    [Min(0)]
    public float MagnetPower;
    [Min(0)]
    public float MagnetDistance;

    [Min(0)]
    private float powerUpLevel = 0;

    public float PowerUpLevel
    {
        get
        {
            return powerUpLevel;
        }

        set
        {
            powerUpLevel = value;
            PowerUpLevelChanged?.Invoke(powerUpLevel);
        }
    }

    #endregion Attributes


    /**
     * Weapons
     */

    #region Weapons
    [SerializeField] private PlayerWeapon[] Weapons;
    [SerializeField] private SpecialAttack specialAttack = null;


    [Space(2)]
    [Range(1, 4)] private int CurrentWeapnType = 0;
    public int PowerUpCollectAmmount = 0;


    private int currentWeapon;
    private int clicktimes;
    private float clicktimer;
    private bool clicked;

    public int getCurrentWeaponType
    {
        get { return currentWeapon; }
    }

    #endregion Weapons


    public override void OnAwake()
    {
        Alive = true;
        shipController = GetComponent<SimpleShipControls>();
        animator = GetComponentInChildren<Animator>();
    }

    public override void ShipSetup()
    {
        for (int i = 0; i < Weapons.Length; i++)
        {
            if (Weapons[i].gameObject.activeSelf)
            {
                Weapons[i].SetShipTransform(transform);
            }
        }

        specialAttack.SetShipTransform(transform);

        PlayerShipHit += DownGradeWeapon;

        SwitchWeapon(0);

        PlayerData playerData = DataController.Instance.GetPlayerData();
        PlayerShipData playerShipData = playerData.GetCurrentPlayerShipData();
        if (playerShipData.Upgrades[((int)UpgradeType.Shield - 1)] == 0)
        {
            HasShield = false;
        }
        else
        {
            HasShield = true;
        }

        ShieldEffect.SetActive(HasShield);

        shipController.Speed = Speed;
    }

    private void Update()
    {
        if (Time.frameCount % 1 == 0)
        {
            if (invisibilityTimer >= 0)
            {
                invisibilityTimer -= Time.deltaTime;
            }
        }

        WeaponSystem();
    }

    public void WeaponSystem()
    {
        float playerPowerUp = 0;

        playerPowerUp = GetPowerUpLevelPresentage();


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
                        clicktimer = 1;
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

        if (PowerUpCollectAmmount >= 5)
        {
            PowerUpCollectAmmount = 0;
            UpgradeWeapon();
        }


        if (CrossPlatformInputManager.GetButtonDown("Fire2") && playerPowerUp >= 1)
        {
            ActivateSpecial();
        }


#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.T))
        {
            PlayerShip playerShip = PlayerManager.GetPlayer();
            if (playerShip != null)
            {
                playerShip.IncreasePowerUp(0.5f);
                PowerUpCollected();
            }
        }
#endif
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
        GameObject explostion = PoolManager.Instance.GetObjectFromPool(ExplostionEffect);
        explostion.transform.position = transform.position;

        PlayerShipDeath?.Invoke();
        gameObject.SetActive(false);
    }

    public override void Heal(float ammount)
    {
        base.Heal(ammount);

        if (HealthPresentage > .2f)
        {
            //AudioManager.Instance.StopSoundEffect();
        }
    }

    public override void TakeDamage(float dmg)
    {
        if (IsDestroyed == false)
        {
            return;
        }

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

                PlayerShipHit?.Invoke();

                if (GotHit == false)
                {
                    PlayerData playerData = DataController.Instance.GetPlayerData();
                    ObjectiveData objectiveData = playerData.GetOnGoingObjectiveById(ObjectiveType.Unharmed);
                    if (objectiveData != null)
                        objectiveData.UpdateProgress(0);
                    GotHit = true;
                }

                if (HealthPresentage < .2f)
                {
                    if (AudioManager.Instance)
                        AudioManager.PlaySound(null, "Alarm", 2);
                }

                if (CurrentHealth < 1)
                {
                    Death();
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        string gameobjectTag = other.gameObject.tag;
        Items items = other.GetComponent<Items>();

        if (items != null)
        {
            items.Action(this);
            if (items.GetItemType().PowerPack)
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
            if (CanUsePowerUpItem)
            {

                AudioManager.PlaySound(null, "Power", 3);
                CurrentWeapnType++;

                if (CurrentWeapnType > 4)
                {
                    CurrentWeapnType = 4;
                }
            }
            else
            {
                IncreasePowerUp(0.1f);
            }

            TempFireRateUpgrade = false;
            SwitchWeapon(CurrentWeapnType);
        }
    }

    public void DownGradeWeapon()
    {
        if (armorUpgrade)
        {
            return;
        }

        if (CurrentWeapnType > 0)
        {
            CurrentWeapnType--;
            SwitchWeapon(CurrentWeapnType);
        }

    }

    public void ResetWeaponPowerUPCollected()
    {
        PowerUpCollectAmmount = 0;
    }

    public void PowerUpCollected()
    {
        if (PowerUpCollectAmmount <= 5 && CurrentWeapnType < 4)
        {
            PowerUpCollectAmmount += 2;
            if (PowerUpCollectAmmount > 5)
            {
                PowerUpCollectAmmount = 5;
            }
            Debug.Log("increase FireRate by " + 0.01f * PowerUpCollectAmmount);
            TempFireRateBuff(0.01f * PowerUpCollectAmmount);
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
        return Weapons[currentWeapon].gameObject;
    }

    public void SwitchWeapon(int WeaponTypeIndex)
    {
        currentWeapon = WeaponTypeIndex;

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

        xpToLevel = 100 * Mathf.Pow(Level, 0.1f) *
              Mathf.Pow(Level, 2) + Mathf.Pow(Level - 1, 4);

        SuperDamage = level * shipStats.baseDamage;

        SuperChargeTime = shipStats.baseSpecialCountdown;

        float[] UpgradeStats = GetCalculatedUpgradeStats();

        Speed += UpgradeStats[0];
        Damage += UpgradeStats[1];
        FireRate -= UpgradeStats[2];
        MagnetPower += UpgradeStats[3];
        MagnetDistance += UpgradeStats[4];
        SuperChargeTime += UpgradeStats[5];
        SuperDamage += UpgradeStats[6];

    }

    public float[] GetCalculatedUpgradeStats()
    {
        var GameControllerSpeedValue = 0;
        var GameControllerDamageValue = 0;
        var GameControllerFireRateValue = 0;
        var GameControllerMagnetPowerValue = 0;
        var GameControllerActivtateDistanceValue = 0;
        var GameControllerSuperTime = 0;
        var GameControllerSuperDamage = 0;
        PlayerData playerData = DataController.Instance.GetPlayerData();
        PlayerShipData playerShipData = playerData.GetCurrentPlayerShipData();

        if (GameManager.Instance)
        {
            GameControllerSpeedValue = playerShipData.Upgrades[(int)UpgradeType.Speed];
            GameControllerDamageValue = playerShipData.Upgrades[(int)UpgradeType.Damage];
            GameControllerFireRateValue = playerShipData.Upgrades[(int)UpgradeType.FireRate];
            GameControllerMagnetPowerValue = playerShipData.Upgrades[(int)UpgradeType.MagnetStrength];
            GameControllerActivtateDistanceValue = playerShipData.Upgrades[(int)UpgradeType.MagnetDistance];
            GameControllerSuperTime = playerShipData.Upgrades[(int)UpgradeType.SuperrechargeTime];
            GameControllerSuperDamage = playerShipData.Upgrades[(int)UpgradeType.SuperDamage];
        }


        var SpeedMultiplier = .1f * GameControllerSpeedValue;
        var DamageMultiplier = 1f * GameControllerDamageValue;
        var FireRateMultiplier = 0.01f * GameControllerFireRateValue;
        var MagnetPowerMultiplier = 1f * GameControllerMagnetPowerValue;
        var MagnetDistanceMultiplier = 1f * GameControllerActivtateDistanceValue;
        var superTime = 0.1f * GameControllerSuperTime;
        var superDamage = 1f * GameControllerSuperDamage;

        return new float[] {
            SpeedMultiplier,
            DamageMultiplier,
            FireRateMultiplier,
            MagnetPowerMultiplier,
            MagnetDistanceMultiplier,superTime,superDamage};
    }

    public void IncreasePowerUp(float value)
    {
        PowerUpLevel += value;
    }

    public float GetPowerUpLevelPresentage()
    {
        return PowerUpLevel;
    }


    #region Upgrade System Methods
    public float GetSpeedUpgrade() { return Upgrades[0]; }
    public float GetDamageUpgrade() { return Upgrades[1]; }
    public float GetFireRateUpgrade() { return Upgrades[2]; }
    public float GetMagnetDistanceUpgrade() { return Upgrades[3]; }
    public float GetMagnetPower()
    {
        return Upgrades[4];
    }
    public void SetUpgrades(float[] Upgrades)
    {
        this.Upgrades = Upgrades;
    }
    public float[] GetUpgrades()
    {
        return Upgrades;
    }
    public void SetPlayerData(PlayerData playerData)
    {
        this.playerData = playerData;
        RefreshUpgradeData();
    }

    public void ApplyUpdatesToShip()
    {
        Speed += Upgrades[0];
        Damage += Upgrades[1];
        FireRate -= Upgrades[2];
        MagnetPower += Upgrades[3];
        MagnetDistance += Upgrades[4];
        SuperChargeTime += Upgrades[5];
        SuperDamage += Upgrades[6];
    }

    public void RefreshUpgradeData()
    {
        PlayerShipData playerShipData = DataController.Instance.GetPlayerData().GetCurrentPlayerShipData();
        GameControllerSpeedValue = playerShipData.Upgrades[(int)UpgradeType.Speed];
        GameControllerDamageValue = playerShipData.Upgrades[(int)UpgradeType.Damage];
        GameControllerFireRateValue = playerShipData.Upgrades[(int)UpgradeType.FireRate];
        GameControllerMagnetPowerValue = playerShipData.Upgrades[(int)UpgradeType.MagnetStrength];
        GameControllerActivtateDistanceValue = playerShipData.Upgrades[(int)UpgradeType.MagnetDistance];
        GameControllerSuperTime = playerShipData.Upgrades[(int)UpgradeType.SuperrechargeTime];
        GameControllerSuperDamage = playerShipData.Upgrades[(int)UpgradeType.SuperDamage];


        if (playerShipData.Upgrades[(int)UpgradeType.ArmorUpgrade - 1] == 1)
        {
            armorUpgrade = true;
        }
        else
        {
            armorUpgrade = false;
        }

        var SpeedMultiplier = .1f * GameControllerSpeedValue;
        var DamageMultiplier = 1f * GameControllerDamageValue;
        var FireRateMultiplier = 0.01f * GameControllerFireRateValue;
        var MagnetPowerMultiplier = 1f * GameControllerMagnetPowerValue;
        var MagnetDistanceMultiplier = 1f * GameControllerActivtateDistanceValue;
        var superTime = 0.1f * GameControllerSuperTime;
        var superDamage = 1f * GameControllerSuperDamage;

        Upgrades = new float[] {
            SpeedMultiplier,
            DamageMultiplier,
            FireRateMultiplier,
            MagnetPowerMultiplier,
            MagnetDistanceMultiplier,
            superTime,
            superDamage };
    }
    #endregion
}
