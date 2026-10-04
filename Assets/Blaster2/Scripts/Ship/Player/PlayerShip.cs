using System;
using System.Collections;
using TheGamerUrso.Core;
using UnityEditor.Build.Reporting;
using UnityEditor.MPE;
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
    protected float waitEntry = 2;
    protected float hitEffectTimer;
    protected float delaytEntry = .5f;
    [SerializeField] protected AudioClip alarmSFX;
    [SerializeField] protected GameObject hitEffect;
    protected IEventService eventService;
    //=================================================================================
    public void Start()
    {
        eventService = GameContext.Get<IEventService>();
        healthComponent.OnHealthChanged += OnHealthValueChanged;

    }
    //=================================================================================
    private void OnDestroy()
    {
        healthComponent.OnHealthChanged -= OnHealthValueChanged;
        ((PlayerShipData)shipData).OnPowerPackCollected -= ShipData_OnPowerPackCollected;
        ((PlayerShipData)shipData).OnLevelUp -= ShipData_OnLevelUpHandled;
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

        if (!hitEffect.activeInHierarchy) return;

        hitEffectTimer -= Time.deltaTime;
        if (hitEffectTimer <= 0)
        {
            hitEffect.SetActive(false);
        }
    }
    //=================================================================================
    public void Setup(PlayerShipData playerShipData)
    {
        shipData = playerShipData;

        SetStats(shipData.Level);

        ApplyUpgrades(playerShipData.GetCalculatedUpgradeStats());
        healthComponent.Setup(this);
        weaponController.Setup(this);
        weaponController.SetWeapon(0);

        ((PlayerShipData)shipData).OnPowerPackCollected += ShipData_OnPowerPackCollected;
        ((PlayerShipData)shipData).OnLevelUp += ShipData_OnLevelUpHandled;

        if (GetPlayerShipData().HasShield)

            ActiveShield();
        else
            DeactivateShield();
    }

    private void ShipData_OnLevelUpHandled(int obj)
    {
        eventService.Publish<PlayerStatsUpdatedEvent>(new PlayerStatsUpdatedEvent() { type = PlayerStatsUpdatedEvent.StatType.Level, value = shipData.Level });
        SetStats(shipData.Level);
    }

    //=================================================================================
    public void Heal(float amount)
    {
        healthComponent.Heal(amount * shipData.Level);
    }
    //=================================================================================
    private void OnHealthValueChanged(float currentHealth, float maxHealth)
    {
        var healthPresentage = currentHealth / maxHealth;
        if (!((PlayerShipData)shipData).HasArmorUpgrade)
        {
            ((PlayerWeaponController)weaponController).DowngradeWeapon();
        }

        if (healthPresentage < .5f) audioSource.PlayOneShot(alarmSFX);

        if (currentHealth < 1) 
            Death();
        if (shipData.HasShield) 
            DeactivateShield();

        Hit();
    }
    //=================================================================================
    public override void Death()
    {
        currentPlayerState = PlayerStateEnum.Death;
          var explostion = PoolManager.Instance.GetObjectFromPool(ship_SO.ExplostionEffect);
        explostion.transform.position = transform.position;
        explostion.SetActive(true);

        eventService.Publish(new ShakeCameraEvent());
        gameObject.SetActive(false);
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

    public override void SetStats(int level)
    {
        var playerData = (PlayerShipData)shipData;
        var player_SO = (Player_SO)ship_SO;

        playerData.Level = Mathf.Clamp(level, 1, 10);

        playerData.Health = level * player_SO.baseHealth;
        playerData.Speed = ship_SO.baseSpeed;
        playerData.Damage = (level * player_SO.baseDamage);
        playerData.FireRate = player_SO.baseFireRate;

        GetPlayerShipData().SuperDamage = (level * player_SO.baseSuperDamage);
        GetPlayerShipData().SuperChargeTime = player_SO.baseSpecialCountdown;

        healthComponent.Setup(this);
        movementController.Setup(this);
        weaponController.Setup(this);
    }
    //=================================================================================
    public void ApplyUpgrades(float[] UpgradeStats)
    {
        shipData.Speed += UpgradeStats[(int)UpgradeTypeEnum.Speed];
        shipData.Damage += UpgradeStats[(int)UpgradeTypeEnum.Damage];
        shipData.FireRate -= UpgradeStats[(int)UpgradeTypeEnum.FireRate];
        GetPlayerShipData().SuperDamage += UpgradeStats[(int)UpgradeTypeEnum.SuperDamage];
        GetPlayerShipData().SuperChargeTime -= UpgradeStats[(int)UpgradeTypeEnum.SuperrechargeTime];
        GetPlayerShipData().MagnetPower = UpgradeStats[(int)UpgradeTypeEnum.MagnetStrength];
        GetPlayerShipData().MagnetDistance = UpgradeStats[(int)UpgradeTypeEnum.MagnetDistance];
        GetPlayerShipData().HasArmorUpgrade = UpgradeStats[(int)UpgradeTypeEnum.ArmorUpgrade] == 1 ? true : false;
        GetPlayerShipData().HasShield = UpgradeStats[(int)UpgradeTypeEnum.Shield] == 1 ? true : false;
    }
    

    public PlayerShipData GetPlayerShipData()
    {
        return ((PlayerShipData)shipData);
    }
    public Player_SO GetPlayerSO()
    {
        return ((Player_SO)ship_SO);
    }
    public override void Hit()
    {
        hitEffect.SetActive(true);
        hitEffectTimer = .5f;
    }
    private void ShipData_OnPowerPackCollected(float obj)
    {
        if (GetPlayerShipData().PowerPackCollected >= 5)
        {
            ((PlayerWeaponController)weaponController).UpgradeWeapon();
        }
    }
}
