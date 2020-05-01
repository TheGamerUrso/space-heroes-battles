using System;
using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.PoolSystem;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerShip : Ship, IDamagable
{
    public Action PlayerShipHit;
    public Action PlayerShipDeath;

    private PlayerData playerData;
    private PlayerShipData playerShipData;

    [Header("Player Config")]
    private SimpleShipControls shipController;

    [Header("Extra Effects")]
    public ParticleSystem ItemCollectedEffect;

    private float invisibilityTimer;
    public bool TempFireRateUpgrade { get; set; }
    /**
     * Weapons
     */

    #region Weapons
    [SerializeField] private PlayerWeapon[] Weapons;
    [SerializeField] private SpecialAttack specialAttack;


    [Space(2)]
    [Range(1, 4)] private int CurrentWeapnType = 0;
    public int getCurrentWeaponType
    {
        get { return CurrentWeapnType; }
    }

    private int clicktimes;
    private float clicktimer;
    private bool clicked;
    #endregion Weapons

    private void OnDestroy()
    {
        playerData.OnLevelValueChanged -= OnLevelValueChanged;
    }

    public override void OnAwake()
    {
        Alive = true;
        shipController = GetComponent<SimpleShipControls>();
        animator = GetComponentInChildren<Animator>();
        playerData = PersistantData.GetPlayerData();
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

        playerData = PersistantData.GetPlayerData();

        if (playerData == null)
        {
            playerData = new PlayerData();
        }

        playerShipData = playerData.GetCurrentPlayerShipData();

        HasShield = playerShipData.HasShield;

        ShieldEffect.SetActive(HasShield);

        SetStats(playerShipData.level);

        playerData.powerUpLevel = 0;
        playerData.powerPackCollected = 0;

        playerData.OnLevelValueChanged += OnLevelValueChanged;
    }
    public void OnLevelValueChanged(int Level)
    {
        SetStats(Level);
    }

    private void Update()
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

    public void WeaponSystem()
    {
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

        if (playerData.powerPackCollected >= 5)
        {
            playerData.powerPackCollected = 0;
            UpgradeWeapon();
        }


        if (CrossPlatformInputManager.GetButtonDown("Fire2") && playerPowerUp >= 1)
        {
            ActivateSpecial();
        }


#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Q))
        {
            PlayerData playerData = PersistantData.GetPlayerData();

            if (playerData != null)
            {
                playerData.IncreasePowerUp(1);
            }

            PowerUpCollected();

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
        if (IsAlive == false)
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

                if (playerData.GotHitInGame == false)
                {
                    PlayerData playerData = PersistantData.GetPlayerData();
                    ObjectiveData objectiveData = playerData.GetOnGoingObjectiveById(ObjectiveType.Unharmed);
                    if (objectiveData != null)
                        objectiveData.UpdateProgress(0);
                    playerData.GotHitInGame = true;
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

    public override void Enter()
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
            if (shipStats.CanUsePowerUpItem)
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
                PlayerData playerData = PersistantData.GetPlayerData();

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
            //Debug.Log("increase FireRate by " + 0.01f * playerData.PowerUpCollectAmmount);
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

        Speed += UpgradeStats[(int)UpgradeType.Speed];
        //Debug.Log("Speed:" + Speed);
        Damage += UpgradeStats[(int)UpgradeType.Damage];
        //Debug.Log("Damage: " + Damage);
        FireRate -= UpgradeStats[(int)UpgradeType.FireRate];
        //Debug.Log("FireRate: " + FireRate);

        if (Id == 1)
        {
            playerShipData.SuperDamage = (level * shipStats.baseSuperDamage);
            if (UpgradeStats[(int)UpgradeType.SuperDamage] > 0)
            {
                playerShipData.SuperDamage = shipStats.baseSuperDamage * UpgradeStats[(int)UpgradeType.SuperDamage];
            }
        }
        else
        {
            playerShipData.SuperDamage = (level * shipStats.baseSuperDamage) + UpgradeStats[(int)UpgradeType.SuperDamage];
        }
        //Debug.Log("SuperDamage: " + playerShipData.SuperDamage);
        playerShipData.SuperChargeTime = shipStats.baseSpecialCountdown - UpgradeStats[(int)UpgradeType.SuperrechargeTime];
        //Debug.Log("SuperChargeTime: " + playerShipData.SuperChargeTime);
        playerShipData.MagnetPower = UpgradeStats[(int)UpgradeType.MagnetStrength];
        //Debug.Log("MagnetPower: " + playerShipData.MagnetPower);
        playerShipData.MagnetDistance = UpgradeStats[(int)UpgradeType.MagnetDistance];
        //Debug.Log("MagnetDistance: " + playerShipData.MagnetDistance);

    }
}
