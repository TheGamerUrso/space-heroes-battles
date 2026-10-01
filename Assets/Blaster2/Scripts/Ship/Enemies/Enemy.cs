using System;

using TheGamerUrso.Core;
using UnityEngine;


public enum EnemyState
{
    None, Idle, Enter, Combat, Escape, Death
}

public class Enemy : Ship, ITargetable
{
    public EnemyState enemyState;

    public Action<Enemy> OnEnemyAttack;
    public Action<Enemy> OnEnemyEscaped;
    public Action<Enemy> OnEnemyEntered;
    public Enemy_SO EnemyData;
    protected PlayerData playerData;

    public bool Targetable
    {
        get
        {
            return healthComponent.HasShield || healthComponent.IsAlive;
        }
    }

    public PoolGameObjectType GameObjectType { get; set; }
    protected IDataService dataService;
    protected IEventService eventService;
    private float delaytEntry = .5f;
    [SerializeField] protected PoolGameObjectType explostionEffect;

    public virtual void Awake()
    {
        healthComponent.Setup(100, false);
    }

    public virtual void Start()
    {
        eventService = GameContext.Get<IEventService>();
        dataService = GameContext.Get<IDataService>();
        playerData = dataService.GetPlayerData();

        SetState(EnemyState.Idle);
    }

    public virtual void Update()
    {
        switch (enemyState)
        {
            case EnemyState.None:
                break;
            case EnemyState.Idle:
                healthComponent.tempGodMode();
                animator.SetBool("Death", false);
                OnEnemyEntered?.Invoke(this);

                delaytEntry = .5f;
                animator.SetTrigger("Enter");
                enemyState = EnemyState.Enter;

                break;
            case EnemyState.Enter:
                delaytEntry -= Time.deltaTime;
                if (delaytEntry <= 0)
                {
                    healthComponent.SetDamagable(true);
                    weaponController.SetWeapon(0);
                    enemyState = EnemyState.Combat;
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
    }

    public void SetState(EnemyState enemyState)
    {
        this.enemyState = enemyState;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(Constants.PLAYTERTAG))
        {
            other?.GetComponent<IDamagable>()?.TakeDamage(healthComponent.CurrentHealth);
            healthComponent?.TakeDamage(healthComponent.CurrentHealth, true);
        }
    }

    public void SetStats(int level)
    {
        SetStats(level, EnemyData.baseHealth, EnemyData.baseSpeed, EnemyData.baseDamage, EnemyData.baseFireRate);
        var rand = UnityEngine.Random.value;
        if (rand < .2f)
        {
            ActiveShield();
        }
        else
        {
            DeactivateShield();
        }
    }

    public override void SetStats(int level,
        float baseHealth,
        float baseSpeed,
        float baseDamage,
        float baseFireRate,
        float baseSuperDamage = 0,
        float baseSpecialCountdown = 0)
    {
        float healthGrowthRate = 0.25f;
        float damageGrowthRate = 0.18f;

        stats.Level = Mathf.Clamp(level, 1, 10);

        stats.Health = baseHealth * (1f + (healthGrowthRate * (stats.Level - 1)));
        stats.Speed = baseSpeed;
        stats.Damage = baseDamage * (1f + (damageGrowthRate * (stats.Level - 1)));
        stats.FireRate = baseFireRate;

        healthComponent.Setup(stats.Health, false);
        weaponController.Setup(this, stats.Damage, stats.FireRate);
        movementController.SetSpeed(stats.Speed);
    }

    public override void Exit()
    {
        eventService.Publish(new EnemyEscapedEvent()
        {
            enemy = this,
            times = 1,
            value = EnemyData.EnemyValue
        });

        SetState(EnemyState.Escape);
        gameObject.SetActive(false);
    }
    public override void Death()
    {
        animator.SetBool("Death", true);
        SetState(EnemyState.Death);

        eventService.Publish(new EnemyDiedEvent()
        {
            enemy = this,
            Level = stats.Level,
            Value = EnemyData.EnemyValue,
            WasBoss = false
        });

        var explostion = PoolManager.Instance.GetObjectFromPool(explostionEffect);
        explostion.transform.position = transform.position;
        explostion.SetActive(true);

        eventService.Publish(new ShakeCameraEvent());
        gameObject.SetActive(false);

    }

    public override void Hit()
    {
        eventService.Publish(new EnemyHitEvent()
        {
            enemy = this,
            Hits = 1
        });

        if (HasShield)
        {
            ActiveShield();
        }
        else
        {
            DeactivateShield();
        }
    }


    [ContextMenu("Debug_Enemy")]
    public void Debug_SpawnEnemyElement()
    {
        BaseEnemyMovement enemyMovement = GetComponent<BaseEnemyMovement>();
        enemyMovement.Setup(new Vector3(0, 0, 150), Quaternion.Euler(new Vector3(0, 180, 0)));
        SetState(EnemyState.Idle);
        SetStats(stats.Level, EnemyData.baseHealth, EnemyData.baseSpeed, EnemyData.baseDamage, EnemyData.baseFireRate);
        gameObject.SetActive(true);
    }
}