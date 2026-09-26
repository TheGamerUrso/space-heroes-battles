using System;
using System.Collections;
using TheGamerUrso.Core;
using UnityEditor.Build.Reporting;
using UnityEngine;

public enum PlayerStateEnum
{
    None,Enter,Combat,Death,Exit
}
public class PlayerShip : Ship
{
    public PlayerStateEnum currentPlayerState = PlayerStateEnum.None;

    [SerializeField] private ParticleSystem ItemCollectedEffect;
    [Space()]
    public Player_SO playerStats;
    public bool HasArmorUprade { get; private set; }
    public bool CanUsePowerUpItem { get; private set; }

    float waitEntry = 2;
    float waitExit = 2;

    [SerializeField] private float SuperChargeTime;
    [SerializeField] private float SuperDamage;
    [SerializeField] private float MagnetPower;
    [SerializeField] private float MagnetDistance;

    //=================================================================================
    public void Start()
    {
        healthComponent.OnHealthChanged += OnHealthValueChanged;
    }
    //=================================================================================
    private void OnDestroy()
    {
        healthComponent.OnHealthChanged -= OnHealthValueChanged;
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
        }
    }
    //=================================================================================
    public void Setup(PlayerData playerData, PlayerShipData playerShipData)
    {
        SetStats(playerShipData.level, playerShipData.Health, playerShipData.Speed, playerShipData.Damage, playerShipData.FireRate);
        healthComponent.Setup(playerStats.baseHealth, playerShipData.HasShield);
        ((PlayerWeaponController)weaponController).Setup(this, stats.Damage, stats.FireRate);
        HasArmorUprade = playerShipData.HasArmorUpgrade;
        SetStats(playerShipData.level,
            playerShipData.Health,
            playerShipData.Speed,
            playerShipData.Damage,
            playerShipData.FireRate,
            playerShipData.GetCalculatedUpgradeStats(),
            playerShipData.SuperDamage,
            playerShipData.SuperChargeTime);
    }
    //=================================================================================
    public void ActiveSpecial()
    {
        ((PlayerWeaponController)weaponController).ActivateSpecial();
    }
    //=================================================================================
    public void UpgradeWeapon()
    {
        ((PlayerWeaponController)weaponController).UpgradeWeapon();
    }
    //=================================================================================
    public void Heal(float amount)
    {
        healthComponent.Heal(amount * stats.Level);
    }
    //=================================================================================
    private void OnHealthValueChanged(float currentHealth, float maxHealth)
    {
        var healthPresentage = currentHealth / maxHealth;
        if (!HasArmorUprade)
        {
            ((PlayerWeaponController)weaponController).DowngradeWeapon();
        }

        PlayerPrefs.SetInt("GOT_HIT", 1);

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
        stats.Level = Mathf.Clamp(level, 1, 10);     

        var health = level * baseHealth;
        stats.Health = baseHealth;
        stats.Speed = baseSpeed;
        stats.Damage = baseDamage;
        stats.FireRate = baseFireRate;
          
        healthComponent.Setup(stats.Health, false);
        weaponController.Setup(this, stats.Damage, stats.FireRate);
        movementController.SetSpeed(stats.Speed);
    }
    //=================================================================================
    public void SetStats(int level, 
        float baseHealth,
        float baseSpeed,
        float baseDamage,
        float baseFireRate,
        float[] UpgradeStats,
        float baseSuperDamage,
        float baseSpecialCountdown)
    {
      

        var Health = level * baseHealth;
        var Speed = baseSpeed + UpgradeStats[(int)UpgradeTypeEnum.Speed];
        var Damage = (level * baseDamage) + UpgradeStats[(int)UpgradeTypeEnum.Damage];
        var FireRate = baseFireRate - UpgradeStats[(int)UpgradeTypeEnum.FireRate];
        SuperDamage = (level * baseSuperDamage) + UpgradeStats[(int)UpgradeTypeEnum.SuperDamage];
        SuperChargeTime = baseSpecialCountdown - UpgradeStats[(int)UpgradeTypeEnum.SuperrechargeTime];
        MagnetPower = UpgradeStats[(int)UpgradeTypeEnum.MagnetStrength];
        MagnetDistance = UpgradeStats[(int)UpgradeTypeEnum.MagnetDistance];
        HasArmorUprade = UpgradeStats[(int)UpgradeTypeEnum.ArmorUpgrade] == 1 ? true : false;
        SetStats(level, Health, Speed, Damage, FireRate);
    }
}
