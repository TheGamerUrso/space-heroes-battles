using System;

using TheGamerUrso.Core;
using UnityEngine;


public enum EnemyState
{
    None, Idle, Enter, Combat, Escape, Death
}

public class Enemy : Ship, ITargetable
{
    public bool Targetable
    {
        get
        {
            return shipData.HasShield || healthComponent.IsAlive;
        }
    }

    public PoolGameObjectType GameObjectType { get; set; }


    public EnemyState EnemyState;
    [SerializeField] protected PoolGameObjectType explostionEffect;
    [SerializeField] protected GameObject hitEffect;
    protected int hitIndex;
    protected int numberOfHits;

    protected float hitEffectTimer;
    protected float delaytEntry = .5f;
    protected IDataService dataService;
    protected IEventService eventService;

    private void Awake()
    {
        SetStats(shipData.Level);
    }

    public virtual void Start()
    {
        eventService = GameContext.Get<IEventService>();
        SetState(EnemyState.Idle);

        healthComponent.OnHealthChanged += OnHealthValueChanged;
    }

    private void OnDestroy()
    {
        healthComponent.OnHealthChanged -= OnHealthValueChanged;
    }

    public virtual void Update()
    {
        switch (EnemyState)
        {
            case EnemyState.None:
                break;
            case EnemyState.Idle:
                healthComponent.tempGodMode();
                animator.SetBool("Death", false);

                delaytEntry = .5f;
                Enter();

                break;
            case EnemyState.Enter:
                delaytEntry -= Time.deltaTime;
                if (delaytEntry <= 0)
                {
                    healthComponent.SetDamagable(true);
                    weaponController.SetWeapon(0);
                    EnemyState = EnemyState.Combat;
                }
                break;
            case EnemyState.Combat:
                weaponController.ShouldAttack = true;
                var currentWeapon = weaponController.GetCurrentWeapon();
                if (currentWeapon != null)
                {
                    currentWeapon.Shoot();
                }
                break;
            case EnemyState.Escape:
                SetState(EnemyState.Idle);
                break;
            case EnemyState.Death:
                SetState(EnemyState.Idle);
                break;
        }

        if (!hitEffect.activeInHierarchy) return;

        hitEffectTimer -= Time.deltaTime;
        if (hitEffectTimer <= 0)
        {
            hitEffect.SetActive(false);
        }
    }

    public void SetState(EnemyState enemyState)
    {
        this.EnemyState = enemyState;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(Constants.PLAYTERTAG))
        {
            other?.GetComponent<IDamagable>()?.TakeDamage(healthComponent.CurrentHealth);
            healthComponent?.TakeDamage(healthComponent.CurrentHealth, true);
        }
    }

    public override void SetStats(int level)
    {
        float healthGrowthRate = 0.25f;
        float damageGrowthRate = 0.18f;

        shipData.Level = Mathf.Clamp(level, 1, 10);

        shipData.Health = ship_SO.baseHealth * (1f + (healthGrowthRate * (shipData.Level - 1)));
        shipData.Speed = ship_SO.baseSpeed;
        shipData.Damage = ship_SO.baseDamage * (1f + (damageGrowthRate * (shipData.Level - 1)));
        shipData.FireRate = ship_SO.baseFireRate;

        var rand = UnityEngine.Random.value;
        if (rand < .2f)
        {
            ActiveShield();
        }

        healthComponent.Setup(this);
        weaponController.Setup(this);
        movementController.Setup(this);
    }


    public override void Enter()
    {
        base.Enter();

        eventService.Publish(new EnemyEvent()
        {
            Type = EnemyEvent.EnemyEventType.ENTER
,
            Enemy = this,
            Value = ship_SO.EnemyValue
        });

        animator.SetTrigger("Enter");
        SetState(EnemyState.Enter);
    }

    public override void Exit()
    {
        eventService.Publish(new EnemyEvent()
        {
            Type = EnemyEvent.EnemyEventType.ESCAPE
         ,
            Enemy = this,
            Value = ship_SO.EnemyValue
        });

        SetState(EnemyState.Escape);
        gameObject.SetActive(false);
    }
    public override void Death()
    {
        eventService.Publish(new EnemyEvent() { 
            Type = EnemyEvent.EnemyEventType.DEATH 
        ,
        Enemy = this,
        Value = ship_SO.EnemyValue
        });

        animator.SetBool("Death", true);
        SetState(EnemyState.Death);

        var explostion = PoolManager.Instance.GetObjectFromPool(ship_SO.ExplostionEffect);
        explostion.transform.position = transform.position;
        explostion.SetActive(true);

        eventService.Publish(new ShakeCameraEvent());
        gameObject.SetActive(false);

    }

    public override void Hit()
    {
        hitEffect.SetActive(true);
        hitEffectTimer = .5f;

        eventService.Publish(new EnemyEvent()
        {
            Type = EnemyEvent.EnemyEventType.HIT     ,
            Enemy = this,
            Value = 1
        });
    }

      private void OnHealthValueChanged(float currentHealth, float maxHealth)
    {
        var healthPresentage = currentHealth / maxHealth;

        if (currentHealth < 1) 
            Death();

        if (shipData.HasShield) 
            DeactivateShield();

        Hit();
    }


    [ContextMenu("Debug_Enemy")]
    public void Debug_SpawnEnemyElement()
    {
        BaseEnemyMovement enemyMovement = GetComponent<BaseEnemyMovement>();
        enemyMovement.Setup(new Vector3(0, 0, 150), Quaternion.Euler(new Vector3(0, 180, 0)));
        SetState(EnemyState.Idle);
        SetStats(shipData.Level);
        gameObject.SetActive(true);
    }
}