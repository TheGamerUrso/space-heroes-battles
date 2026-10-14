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
    public event Action OnShipHit;

    [SerializeField] protected Animator animator;
    public PlayerStateEnum currentPlayerState = PlayerStateEnum.None;

    [SerializeField] private ParticleSystem ItemCollectedEffect; 
    [SerializeField] protected AudioClip alarmSFX;
    [SerializeField] protected GameObject hitEffect;

    protected float delayEntryTimer = 2;
    protected float delayEntry = .5f;
    protected float hitEffectTimer;

    protected IEventService eventService;

    public override ShipData ShipData { get => playerShipData; }

    public override Ship_SO Ship_SO { get => player_SO; }

    [SerializeField] protected PlayerShipData playerShipData;
    [SerializeField] protected Player_SO player_SO;

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
        playerShipData.OnPowerPackCollected -= ShipData_OnPowerPackCollected;
        playerShipData.OnLevelUp -= ShipData_OnLevelUpHandled;
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
        this.playerShipData = playerShipData;

        playerShipData.OnPowerPackCollected += ShipData_OnPowerPackCollected;
        playerShipData.OnLevelUp += ShipData_OnLevelUpHandled;

        SetStats(playerShipData.Level);

        weaponController.SetWeapon(0);


        if (GetPlayerShipData().HasShield)

            ActiveShield();
        else
            DeactivateShield();
    }

    private void ShipData_OnLevelUpHandled(int obj)
    {
        eventService.Publish<PlayerStatsUpdatedEvent>(new PlayerStatsUpdatedEvent() { type = PlayerStatsUpdatedEvent.StatType.Level, value = playerShipData.Level });
        SetStats(playerShipData.Level);
    }

    //=================================================================================
    public void Heal(float amount)
    {
        healthComponent.Heal(amount * playerShipData.Level);
    }
    //=================================================================================
    private void OnHealthValueChanged(float currentHealth, float maxHealth)
    {
        var healthPresentage = currentHealth / maxHealth;
        if (!playerShipData.HasArmorUpgrade)
        {
            ((PlayerWeaponController)weaponController).DowngradeWeapon();
        }

        if (healthPresentage < .5f) audioSource.PlayOneShot(alarmSFX);

        if (currentHealth < 1) 
            Death();
        if (playerShipData.HasShield) 
            DeactivateShield();

        Hit();
    }
    //=================================================================================
    public override void Death()
    {
        currentPlayerState = PlayerStateEnum.Death;
          var explostion = PoolManager.Instance.GetObjectFromPool(player_SO.ExplostionEffect);
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
        playerShipData.Level = Mathf.Clamp(level, 1, 10);

        playerShipData.Health = player_SO.baseHealth + (5 * playerShipData.GetUpgrade(UpgradeTypeEnum.Health));
        playerShipData.Speed = player_SO.baseSpeed + (1 * playerShipData.GetUpgrade(UpgradeTypeEnum.Speed));
        playerShipData.FireRate = player_SO.baseFireRate - (0.03f * playerShipData.GetUpgrade(UpgradeTypeEnum.FireRate));
        playerShipData.Damage = player_SO.baseDamage + (2.5f * playerShipData.GetUpgrade(UpgradeTypeEnum.Damage));
        playerShipData.SuperDamage = player_SO.baseSuperDamage + (2.5f * playerShipData.GetUpgrade(UpgradeTypeEnum.SuperDamage));
        playerShipData.SuperChargeTime = player_SO.baseSpecialCountdown + (.2f * playerShipData.GetUpgrade(UpgradeTypeEnum.SuperrechargeTime));
        playerShipData.MagnetPower = 0.9f * playerShipData.GetUpgrade(UpgradeTypeEnum.MagnetDistance);
        playerShipData.MagnetDistance =  0.5f * playerShipData.GetUpgrade(UpgradeTypeEnum.MagnetStrength);

        GetPlayerShipData().HasArmorUpgrade = (int)playerShipData.GetUpgrade(UpgradeTypeEnum.ArmorUpgrade) == 1 ? true : false;
        GetPlayerShipData().HasShield = (int)playerShipData.GetUpgrade(UpgradeTypeEnum.Shield)== 1 ? true : false;

        healthComponent.Setup(this);
        movementController.Setup(this);
        weaponController.Setup(this);
    }

    public PlayerShipData GetPlayerShipData()
    {
        return playerShipData;
    }

    public Player_SO GetPlayerSO()
    {
        return player_SO;
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
