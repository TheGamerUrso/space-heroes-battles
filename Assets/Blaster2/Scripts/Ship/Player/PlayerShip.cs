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
                waitEntry = 2;
                animator.SetTrigger(Constants.PLAYEREXITSTRINGKEY);
                break;
        }
    }
    //=================================================================================
    public void Setup(PlayerData playerData, PlayerShipData playerShipData)
    {
        SetStats(playerShipData.level,
            playerShipData.Health,
            playerShipData.Speed,
            playerShipData.Damage,
            playerShipData.FireRate,
            playerShipData.SuperDamage,
            playerShipData.SuperChargeTime);

        ApplyUpgrades(playerShipData.GetCalculatedUpgradeStats());
        HasArmorUprade = playerShipData.HasArmorUpgrade;
        healthComponent.Setup(playerStats.baseHealth, playerShipData.HasShield);
        ((PlayerWeaponController)weaponController).Setup(this, stats.Damage, stats.FireRate);
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
    public override void SetStats(int level, 
        float baseHealth,
        float baseSpeed,
        float baseDamage,
        float baseFireRate,
        float baseSuperDamage,
        float baseSpecialCountdown)
    {
        stats.Level = Mathf.Clamp(level, 1, 10);

        stats.Health = level * baseHealth;
        stats.Speed = baseSpeed;
        stats.Damage = (level * baseDamage);
        stats.FireRate = baseFireRate;

        healthComponent.Setup(stats.Health, false);
        weaponController.Setup(this, stats.Damage, stats.FireRate);
        movementController.SetSpeed(stats.Speed);
        SuperDamage = (level * baseSuperDamage);
        SuperChargeTime = baseSpecialCountdown;
    }
    //=================================================================================
    public void ApplyUpgrades(float[] UpgradeStats)
    {
        stats.Speed += UpgradeStats[(int)UpgradeTypeEnum.Speed]; 
        stats.Damage += UpgradeStats[(int)UpgradeTypeEnum.Damage];
        stats.FireRate -= UpgradeStats[(int)UpgradeTypeEnum.FireRate];
        SuperDamage += UpgradeStats[(int)UpgradeTypeEnum.SuperDamage];
        SuperChargeTime -= UpgradeStats[(int)UpgradeTypeEnum.SuperrechargeTime];
        MagnetPower = UpgradeStats[(int)UpgradeTypeEnum.MagnetStrength];
        MagnetDistance = UpgradeStats[(int)UpgradeTypeEnum.MagnetDistance];
        HasArmorUprade = UpgradeStats[(int)UpgradeTypeEnum.ArmorUpgrade] == 1 ? true : false;
    }
}
