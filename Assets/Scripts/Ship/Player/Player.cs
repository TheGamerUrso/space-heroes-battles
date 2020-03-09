using System.Collections;
using TheGamerUrso.PoolSystem;
using UnityEngine;

public class Player : Ship, IDestroyable
{

    public int playerID;

    [Header("Player Config")]
    private bool Alive;


    private SimpleShipControls shipController;
    private PlayerWeaponSystem weaponSystem;
    private PlayerAnimation playerAnimation;
    [SerializeField] protected UpgradeSystem upgradeSystem;

    public static bool TempFireRateUpgrade { get; set; }
    public bool CanUsePowerUpItem;


    private float invisibilityTimer;
    private bool GotHit;
    private int SuperUsed;
    public ParticleSystem ItemCollectedEffect;
    public bool IsDestroyed
    {
        get { return Alive; }
        set { Alive = value; }
    }

    public void InitIfNeeded()
    {
        if (shipController == null)
        {
            shipController = GetComponent<SimpleShipControls>();
            weaponSystem = GetComponentInChildren<PlayerWeaponSystem>();
            playerAnimation = GetComponentInChildren<PlayerAnimation>();
        }
    }

    public override void InitReferences()
    {
        InitIfNeeded();
    }

    public void PlayerWeaponSystem_OnSuperUsedHandled()
    {

    }

    public override void ShipStartSetUp()
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


        //TO DO Removed
        //weaponSystem.SetShipStatSystem(shipStatsSystem);

        weaponSystem.SetPlayerAnimation(playerAnimation);
        weaponSystem.SetPlayer(this);
        GameEventSystem.OnPlayerLevelUp += LevelSystem_OnLevelUpHandled;
    }

    public void LevelSystem_OnLevelUpHandled()
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

                ShakeEffect shakeEffect = GameObject.FindObjectOfType<ShakeEffect>();

                if (shakeEffect)
                {
                    shakeEffect.StartEffect();
                }

                if (ComboKillIndicator.instance != null)
                {
                    ComboKillIndicator.instance.ZeroMiltiplier();
                }
                if (upgradeSystem.ArmorUpgradeCheck() == false)
                {
                    PlayerWeaponSystem playerWeaponSystem = GameObject.FindObjectOfType<PlayerWeaponSystem>();
                    playerWeaponSystem.DownGradeWeapon();
                }

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

            items.Action();
            if (items.GetItemType().PowerPack)
                ItemCollectedEffect.Play();
        }
    }

    public PlayerAnimation PlayerAnimation()
    {
        InitIfNeeded();
        return playerAnimation;
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
}