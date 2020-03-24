using System;
using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.PoolSystem;
using UnityEngine;

public class PlayerShip : Ship, IDestroyable
{
    public Action PlayerShipHit;
    public Action PlayerShipDeath;
    public Action<ItemData> PickUpItem;

    [Header("PlayerShip")]
    public int playerID;

    private bool Alive = true;
    public bool IsDestroyed
    {
        get { return Alive; }
        set { Alive = value; }
    }

    [Header("Player Config")]
    [SerializeField] private UpgradeSystem upgradeSystem;
    private SimpleShipControls shipController;
    private PlayerWeaponSystem weaponSystem;

    [Header("Extra Effects")]
    public ParticleSystem ItemCollectedEffect;


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

    public bool HasArmorUpgrade
    {
        get
        {
            return upgradeSystem.ArmorUpgradeCheck();
        }
    }


    private void OnDestroy()
    {
        levelSystem.XPChanged -= XpChangedCallback;
    }

    private void OnEnable()
    {
        levelSystem.XPChanged += XpChangedCallback;
    }

    public override void OnAwake()
    {
        Alive = true;
        shipController = GetComponent<SimpleShipControls>();
        weaponSystem = GetComponentInChildren<PlayerWeaponSystem>();
        animator = GetComponentInChildren<Animator>();
    }


    public override void ShipSetup()
    {
        PlayerData playerData = DataController.GetPlayerData();

        if (playerData.Upgrades[((int)UpgradeType.Shield - 1)] == 0)
        {
            HasShield = false;
        }
        else
        {
            HasShield = true;
        }

        ShieldEffect.SetActive(HasShield);

        shipStatsSystem.SetStats(levelSystem);

        shipController.Speed = shipStatsSystem.Speed;

        upgradeSystem.ApplyUpdatesToShip(shipStatsSystem);
    }

    public void XpChangedCallback(int level, float xp, float xpToLevel)
    {
        shipStatsSystem.SetStats(levelSystem);
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
    }



    public void UpdateWeaponStats(float fireRate, float damage = 0)
    {
        WeaponScript weapon = PlayerWeaponSystem.GetCurrentActiveWeapon(weaponSystem).GetComponent<WeaponScript>();

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

    public void Heal(float ammount)
    {
        CurrentHealth += ammount;

        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }

        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);

        if (GetHealthPresentage() > .2f)
        {
            //AudioManager.Instance.StopSoundEffect();
        }
    }

    public void TakeDamage(float dmg)
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

                PlayerShipHit?.Invoke();

                if (GotHit == false)
                {
                    PlayerData playerData = DataController.GetPlayerData();
                    ObjectiveData objectiveData = playerData.GetOnGoingObjectiveById(ObjectiveType.Unharmed);
                    if (objectiveData != null)
                        objectiveData.UpdateProgress(0);
                    GotHit = true;
                }

                if (GetHealthPresentage() < .2f)
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

    internal void AddXP(int xPEarned)
    {
        levelSystem.AddXP(xPEarned);
    }

    private void OnTriggerEnter(Collider other)
    {
        string gameobjectTag = other.gameObject.tag;
        Items items = other.GetComponent<Items>();

        if (gameobjectTag.Equals(Constants.ENEMYTAG))
        {
            IDestroyable destroyable = other.GetComponent<IDestroyable>();
            if (destroyable != null)
            {
                destroyable.TakeDamage(destroyable.CurrentHealth);
            }
        }

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

    public void PowerUpCollected()
    {
        GetWeaponSystem().PowerUpCollected();
    }

    #region TempfireRateBuff

    public void TempFireRateBuff(float fireRate = 0.0f, bool temporary = false)
    {

        if (!TempFireRateUpgrade)
        {
            TempFireRateUpgrade = true;
            GiveTemporaryFireRateBuff();
        }


        UpdateWeaponStats(shipStatsSystem.FireRate - fireRate);

    }

    public void GiveTemporaryFireRateBuff()
    {
        StartCoroutine(TemporaryFireRateUpgrade());
    }

    public IEnumerator TemporaryFireRateUpgrade()
    {
        var fireRateTemp = shipStatsSystem.FireRate;
        var DamageTemp = shipStatsSystem.Damage;

        while (TempFireRateUpgrade)
        {
            yield return new WaitForEndOfFrame();
        }

        shipStatsSystem.FireRate = fireRateTemp;
        UpdateWeaponStats(shipStatsSystem.FireRate, DamageTemp);
    }

    #endregion

    #region Getters and Setters

    public PlayerWeaponSystem GetWeaponSystem()
    {
        return weaponSystem;
    }

    public UpgradeSystem GetUpgradeSystem()
    {
        return upgradeSystem;
    }

    #endregion




  
}
