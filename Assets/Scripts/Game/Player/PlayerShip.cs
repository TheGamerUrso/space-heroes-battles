using System;
using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.PoolSystem;
using UnityEngine;

public class PlayerShip : Ship, IDestroyable, IEndGameObserver
{
    public Action PlayerShipHit;
    public Action PlayerShipDeath;
    public Action<ItemData> PickUpItem;


    [Header("PlayerShip")]
    public int playerID;
    public Player player;

    private bool Alive;
    public bool IsDestroyed
    {
        get { return Alive; }
        set { Alive = value; }
    }

    [Header("Player Config")]
    [SerializeField] protected UpgradeSystem upgradeSystem;
    private SimpleShipControls shipController;
    private PlayerWeaponSystem weaponSystem;

    public static bool TempFireRateUpgrade { get; set; }
    public bool CanUsePowerUpItem;

    [Header("Extra Effects")]
    public ParticleSystem ItemCollectedEffect;


    private float invisibilityTimer;
    private bool GotHit;
    private int SuperUsed;


    private void OnDestroy()
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.RemoveObserver(this);
        }

        GameEventSystem.XpChanged -= XpChangedCallback;
    }

    private void OnEnable()
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.AddObserver(this);
        }

        GameEventSystem.XpChanged += XpChangedCallback;
        GameEventSystem.XpChanged += XpChangedCallback;
    }

    public void InitIfNeeded()
    {
        if (shipController == null)
        {

            shipController = GetComponent<SimpleShipControls>();
            weaponSystem = GetComponentInChildren<PlayerWeaponSystem>();
        }
    }

    public override void InitReferences()
    {
        shipController = GetComponent<SimpleShipControls>();
        weaponSystem = GetComponentInChildren<PlayerWeaponSystem>();
        animator = GetComponentInChildren<Animator>();
    }


    public override void ShipSetup()
    {
        InitIfNeeded();

        IsDestroyed = true;

        PlayerData playerData = DataController.GetPlayerData();

        if (playerData.Upgrades[((int)UpgradeType.Shield - 1)] == 0)
        {
            bShieldModuleInstalled = false;
        }
        else
        {
            bShieldModuleInstalled = true;
        }

        ShieldEffect.SetActive(bShieldModuleInstalled);

        shipStatsSystem.SetStats(levelSystem);

        shipController.speed = shipStatsSystem.Speed;


        upgradeSystem.ApplyUpdatesToShip(shipStatsSystem);

        weaponSystem.SetPlayer(this);


    }

    public void XpChangedCallback(int level, float xp, float xpToLevel)
    {
        Heal(MaxHealth);
        shipStatsSystem.SetStats(levelSystem);
        weaponSystem.SetPlayer(this);
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
        ShieldEffect.SetActive(bShieldModuleInstalled);
    }

    public override void Death()
    {
        if (GuiManager.Instance)
            GuiManager.Instance.GameOver();

        GameObject explostion = PoolManager.Instance.GetObjectFromPool(ExplostionEffect);
        explostion.transform.position = transform.position;



        gameObject.SetActive(false);
    }

    public void Heal(float ammount)
    {

        shipStatsSystem.CurrentHealth += ammount;

        if (shipStatsSystem.CurrentHealth > MaxHealth)
        {
            shipStatsSystem.CurrentHealth = MaxHealth;
        }

        shipStatsSystem.CurrentHealth = Mathf.Clamp(shipStatsSystem.CurrentHealth, 0, MaxHealth);

        if (GetHealthPresentage() > .2f)
        {
            //AudioManager.Instance.StopSoundEffect();
        }
    }

    public void TakeDamage(float dmg)
    {
        InitIfNeeded();
        if (animator != null && animator.GetCurrentAnimatorStateInfo(0).IsName("Enter") || animator.GetCurrentAnimatorStateInfo(0).IsName("Exit"))
        {
            return;
        }

        if (IsDestroyed == false)
        {
            return;
        }

        if (dmg >= MaxHealth)
        {
            dmg = MaxHealth - 1;
        }

        if (bShieldModuleInstalled == true)
        {
            bShieldModuleInstalled = false;
            ShieldEffect.SetActive(bShieldModuleInstalled);
        }
        else if (bShieldModuleInstalled == false)
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

    public PlayerWeaponSystem GetWeaponSystem()
    {
        InitIfNeeded();
        return weaponSystem;
    }

    public UpgradeSystem GetUpgradeSystem()
    {
        return upgradeSystem;
    }

    public bool IsPlayerDamaged()
    {
        return GotHit;
    }

    public void tempGodMode()
    {
        invisibilityTimer = 1;
    }

    public void GameOver()
    {
        animator.SetTrigger(Constants.PLAYEREXITSTRINGKEY);
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
        animator.SetTrigger("Exit");
    }


    public bool HasArmorUpgrade()
    {
        return upgradeSystem.ArmorUpgradeCheck();
    }
}
