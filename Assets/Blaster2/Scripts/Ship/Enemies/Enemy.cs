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
    private float delaytEntry = 1f;
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
                OnEnemyEntered?.Invoke(this);
                enemyState = EnemyState.Enter;
                break;
            case EnemyState.Enter:
                delaytEntry -= Time.deltaTime;
                if (delaytEntry <= 0)
                {
                    delaytEntry = 2;
                    healthComponent.SetDamagable(true);
                    weaponController.SetWeapon(0);
                    enemyState = EnemyState.Combat;
                }
                break;
            case EnemyState.Combat:
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
            var ship = other.GetComponent<IDamagable>();
            ship.TakeDamage(stats.Health / 2);
            healthComponent.TakeDamage(stats.Health / 2);
        }
    }

    public void SetStats(int level)
    {
        base.SetStats(level, EnemyData.baseHealth, EnemyData.baseSpeed, EnemyData.baseDamage, EnemyData.baseFireRate);
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
    }
}