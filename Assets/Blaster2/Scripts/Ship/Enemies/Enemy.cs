using System;
using System.Collections;
using TheGamerUrso.Core;
using UnityEditor.MPE;
using UnityEngine;

public enum EnemyState
{
    None,Idle,Enter,Combat,Hit,Escape,Death
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

    [HideInInspector] public EnemyElement enemyElement;
    protected IDataService dataService;
    protected IEventService eventService;
    private float delaytEntry = 2;

    public virtual void Awake()
    {
        healthComponent.Setup(100, false);
    }

    public virtual void Start()
    {
        eventService = GameContext.Get<IEventService>();
        dataService = GameContext.Get<IDataService>();
        playerData = dataService.GetPlayerData();
 
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
            case EnemyState.Hit:
                break;
            case EnemyState.Escape:
                OnEnemyEscaped?.Invoke(this);
                gameObject.SetActive(false);
                break;
            case EnemyState.Death:
                gameObject.SetActive(false);
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
            ship.TakeDamage(Health / 2);
            healthComponent.TakeDamage(Health / 2);
        }
    }
}