using System;
using System.Collections;
using TheGamerUrso.Core;
using UnityEngine;

public enum PlayerStateEnum
{
    None,Enter,Combat,Death,Exit
}
public class PlayerShip : Ship
{
    public Action<int> OnItemPickedUp;


    public PlayerStateEnum currentPlayerState = PlayerStateEnum.None;

    [SerializeField] private ParticleSystem ItemCollectedEffect;
    [Space()]
    [SerializeField] private Player_SO playerStats;

    private PlayerData playerData;
    private PlayerShipData playerShipData;
    public bool HasArmorUprade { get; private set; }
    public bool CanUsePowerUpItem { get; private set; }
    private IDataService dataService;

    float waitEntry = 2;
    float waitExit = 2;


    //=================================================================================
    public void Start()
    {
        dataService = GameContext.Get<IDataService>();

        playerData = dataService.GetPlayerData();
        playerShipData = playerData.GetCurrentPlayerShipData();

        SetStats(playerShipData.level,playerShipData.Health,playerShipData.Speed,playerShipData.Damage,playerShipData.FireRate);



        healthComponent.Setup(100, playerShipData.HasShield);

        healthComponent.OnHealthChanged += OnHealthValueChanged;
        ((PlayerWeaponController)weaponController).Initialize(this, Damage, FireRate);
    }

    //=================================================================================
    public void Update()
    {
        switch (currentPlayerState)
        {
            case PlayerStateEnum.None:
               
                currentPlayerState = PlayerStateEnum.Enter;
                break;
            case PlayerStateEnum.Enter:
                animator.SetTrigger(Constants.PLAYERENTERSTRINGKEY);
                waitEntry -= Time.deltaTime;
                if (waitEntry <= 0)
                {
                    currentPlayerState = PlayerStateEnum.Combat;
                }
                break;
            case PlayerStateEnum.Combat:

                
               
                break;
            case PlayerStateEnum.Death:

                break;
            case PlayerStateEnum.Exit:
                animator.SetTrigger(Constants.PLAYEREXITSTRINGKEY);
                break;
            default:
                break;
        }
    }
    public void ActiveSpecial()
    {
        ((PlayerWeaponController)weaponController).ActivateSpecial();
    }

    public void UpgradeWeapon()
    {
        ((PlayerWeaponController)weaponController).UpgradeWeapon();
    }
    //=================================================================================
    private void OnHealthValueChanged(float currentHealth, float maxHealth)
    {
        var healthPresentage = currentHealth / maxHealth;
        if (!HasArmorUprade)
        {
            ((PlayerWeaponController)weaponController).DowngradeWeapon();
        }

        playerData.GotHitInGame = true;

        if (healthPresentage < .5f)
        {
            audioSource.PlayOneShot(playerStats.alarmSFX);
        }
    }

    //=================================================================================
    public void OnTriggerEnter(Collider other)
    {
        IPickable items = other.GetComponent<IPickable>();
        if (items != null)
        {
            items.PickUp();
            if (items.ID == ItemEnum.POWERUP)
                ItemCollectedEffect.Play();
        }

        if (other.gameObject.tag.Equals(Constants.ENEMYTAG))
        {
            var damagable = other.gameObject.GetComponent<IDamagable>();
            damagable.TakeDamage(damagable.CurrentHealth);
           healthComponent.TakeDamage(damagable.CurrentHealth);
        }
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
    public override void SetStats(int level, float baseHealth, float baseSpeed, float baseDamage, float baseFireRate)
    {
        Level = Mathf.Clamp(level, 1, 10);
        

        var health = level * playerStats.baseHealth;
        float[] UpgradeStats = playerShipData.GetCalculatedUpgradeStats();
        playerShipData.Health = level * playerStats.baseHealth;
        playerShipData.Speed = playerStats.baseSpeed + UpgradeStats[(int)UpgradeTypeEnum.Speed];
        playerShipData.Damage = (level * playerStats.baseDamage) + UpgradeStats[(int)UpgradeTypeEnum.Damage];
        playerShipData.FireRate = playerStats.baseFireRate - UpgradeStats[(int)UpgradeTypeEnum.FireRate];
        playerShipData.SuperDamage = (level * playerStats.baseSuperDamage) + UpgradeStats[(int)UpgradeTypeEnum.SuperDamage];
        playerShipData.SuperChargeTime = playerStats.baseSpecialCountdown - UpgradeStats[(int)UpgradeTypeEnum.SuperrechargeTime];
        playerShipData.MagnetPower = UpgradeStats[(int)UpgradeTypeEnum.MagnetStrength];
        playerShipData.MagnetDistance = UpgradeStats[(int)UpgradeTypeEnum.MagnetDistance];
        HasArmorUprade = UpgradeStats[(int)UpgradeTypeEnum.ArmorUpgrade] == 1 ? true : false;

        Health = playerShipData.Health;
        Speed = playerShipData.Speed;
        Damage = playerShipData.Damage;
        FireRate = playerShipData.FireRate;

       
        healthComponent.Setup(Health, false);
        weaponController.Initialize(this,Damage, FireRate);
        movementController.SetSpeed(Speed);
    }
    //=================================================================================
    public void PowerUpCollected()
    {
        if (playerData.PowerPackCollected <= 5 && weaponController.CurrentWeapnType < 4)
        {
            playerData.SetPowerPackCollected(2);
            ((PlayerWeaponController)weaponController).TempFireRateBuff(0.01f * playerData.PowerPackCollected);
        }
        OnItemPickedUp?.Invoke(1);
    }
}
