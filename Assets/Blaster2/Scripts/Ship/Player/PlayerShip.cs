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
    [SerializeField] protected Animator animator;
    public PlayerStateEnum currentPlayerState = PlayerStateEnum.None;

    [SerializeField] private ParticleSystem ItemCollectedEffect; 
    [SerializeField] protected AudioClip alarmSFX;
    [SerializeField] protected GameObject hitEffect;

    protected float delayEntryTimer = 2;
    protected float delayEntry = .5f;
    protected float hitEffectTimer;

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
                delayEntryTimer -= Time.deltaTime;
                if (delayEntryTimer <= 0)
                {
                    currentPlayerState = PlayerStateEnum.Combat;
                }
                break;
            case PlayerStateEnum.Combat:             
                break;
            case PlayerStateEnum.Death:                
                break;
            case PlayerStateEnum.Exit:
                delayEntryTimer = delayEntry;
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
  
        ((PlayerShipData)shipData).OnPowerPackCollected += ShipData_OnPowerPackCollected;
        ((PlayerShipData)shipData).OnLevelUp += ShipData_OnLevelUpHandled;

        SetStats(shipData.Level);

        weaponController.SetWeapon(0);


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

        playerData.Health = player_SO.baseHealth;
        playerData.Speed = ship_SO.baseSpeed;
        playerData.FireRate = player_SO.baseFireRate;
        playerData.Damage = player_SO.baseDamage;
        playerData.SuperDamage = player_SO.baseSuperDamage;
        playerData.SuperChargeTime = player_SO.baseSpecialCountdown;
        playerData.MagnetPower = player_SO.baseSpecialCountdown;
        playerData.MagnetDistance = player_SO.baseSpecialCountdown;    

        shipData.Health += playerData.GetUpgrade(UpgradeTypeEnum.Health); 
        shipData.Speed += playerData.GetUpgrade(UpgradeTypeEnum.Speed);
        shipData.Damage += playerData.GetUpgrade(UpgradeTypeEnum.Damage);
        shipData.FireRate -= playerData.GetUpgrade(UpgradeTypeEnum.FireRate);

        GetPlayerShipData().SuperDamage += playerData.GetUpgrade(UpgradeTypeEnum.SuperDamage);
        GetPlayerShipData().SuperChargeTime -= playerData.GetUpgrade(UpgradeTypeEnum.SuperrechargeTime);
        GetPlayerShipData().MagnetPower = playerData.GetUpgrade(UpgradeTypeEnum.MagnetStrength);
        GetPlayerShipData().MagnetDistance = playerData.GetUpgrade(UpgradeTypeEnum.MagnetDistance);

        GetPlayerShipData().HasArmorUpgrade = (int)playerData.GetUpgrade(UpgradeTypeEnum.ArmorUpgrade) == 1 ? true : false;
        GetPlayerShipData().HasShield = (int)playerData.GetUpgrade(UpgradeTypeEnum.Shield)== 1 ? true : false;

        healthComponent.Setup(this);
        movementController.Setup(this);
        weaponController.Setup(this);
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
    public PlayerWeaponController GetWeaponController()
    {
        return ((PlayerWeaponController)weaponController);
    }
    private void ShipData_OnPowerPackCollected(float obj)
    {
        if (GetPlayerShipData().PowerPackCollected >= 5)
        {
            ((PlayerWeaponController)weaponController).UpgradeWeapon();
        }
    }
}
