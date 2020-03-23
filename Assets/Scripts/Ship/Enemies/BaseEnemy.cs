using System;
using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.PoolSystem;
using UnityEngine;
using DG.Tweening;

public class BaseEnemy : Ship, IDestroyable
{
    public Action<string, BaseEnemy> EnemyDied;
    public Action<string, BaseEnemy> EnemyGotHit;
    public Action<string, BaseEnemy> EnemyEscaped;

    protected string id;

    [Header("Enemy Config")]
    public BaseEnemyAI baseEnemyAI;
    protected BoxCollider boxCollider;

    protected bool CanAttack;
    [SerializeField]
    protected WeaponScript[] Weapons;
    [SerializeField]
    protected float delayAttak = 3;
    public float DeathDelay;
    protected int currentWeaponActive;



    [HideInInspector] public EnemyElement enemyElement;

    [SerializeField] private HealthBarSettings HealthBarSettings ;
    private EnemyHealthWidget healthBar;

    //IDestroyable Values

    private bool Alive;
    public bool IsDestroyed
    {
        get
        {
            return Alive;
        }
        set
        {
            Alive = value;
        }
    }

    public int m_ValueOfEnemy;

    public PoolGameObjectType[] DropItems;

    protected WeaponScript weaponScript;
    protected float takeDamageDelay;
    protected bool EnableShield;
    [SerializeField] protected bool AutoEnableWeapon;

    private void OnDisable()
    {
        RemoveAndDestroy();
    }

    public virtual void OnEnable()
    {
        Alive = true;
        DisableWeapons();
        GameController gameController = GameObject.FindObjectOfType<GameController>();
        gameController.AddEnemy(this);

        if (AutoEnableWeapon)
        {
            EnableWeapon();
        }
    }

    public void EnableWeapon()
    {
        for (int i = 0; i < Weapons.Length; i++)
        {
            Weapons[i].AutoAttack = true;
        }
    }

    public void DisableWeapons()
    {
        for (int i = 0; i < Weapons.Length; i++)
        {
            Weapons[i].AutoAttack = false;
        }
    }

    public virtual void SetEnemyStats(int level)
    {
        GetLevelSystem().SetLevel(level);
        GetShipStatsSystem().SetStats(levelSystem);
        bShieldModuleInstalled = false;

        if (weaponScript)
        {
            weaponScript.SetShip(this);
        }
    }

    public override void InitReferences()
    {
        shipStatsSystem.SetStats(levelSystem);
        bShieldModuleInstalled = false;
        boxCollider = GetComponent<BoxCollider>();
        animator = GetComponentInChildren<Animator>();
    }

    public override void ShipSetup()
    {
        if (healthBar != null)
        {
            healthBar.GetComponent<BaseHealthWidget>();
        }

        baseEnemyAI = GetComponent<SimpleAI>();
        ShieldEffect.SetActive(bShieldModuleInstalled);

        if (HealthBarSettings != null)
        {
            GameObject initializedHealthWidget = Instantiate(HealthBarSettings.HealthBarPrefab, transform, false);
            healthBar = (EnemyHealthWidget)initializedHealthWidget.GetComponent<BaseHealthWidget>();
            healthBar.Initiallize(this);
        }
    }
    public virtual void Attack() { }

    public virtual void Enter() { }

    public void Leave()
    {
        EnemyEscaped?.Invoke(id, this);
    }

    public virtual void Heal(float ammount)
    {
       CurrentHealth += ammount;

        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }

        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
    }

    public virtual void TakeDamage(float dmg)
    {
        if (Alive == false)
        {
            return;
        }

        if (takeDamageDelay <= 0)
        {
            takeDamageDelay = .1f;

            if (bShieldModuleInstalled == true)
            {
                bShieldModuleInstalled = false;
                ShieldEffect.SetActive(bShieldModuleInstalled);
            }
            else if (bShieldModuleInstalled == false)
            {
                CurrentHealth -= dmg;

                if (CurrentHealth < 1)
                {
                    Death();
                }
            }

        }

        Hit();
    }

    public virtual void Hit()
    {
        EnemyGotHit?.Invoke(id, this);
        //GameEventSystem.Call(GameEventType.Enemy_Hit, id, this);
    }

    public virtual void Update()
    {
        if (takeDamageDelay >= 0)
        {
            takeDamageDelay -= Time.deltaTime;
        }
        Tick();
    }

    public virtual void Tick() { }

    public override void Death()
    {
        if (Alive)
        {
            Alive = false;

            GameObject explostion = PoolManager.Instance.GetObjectFromPool(PoolGameObjectType.ShipExplosion);
            explostion.transform.position = transform.position;

            EnemyDied?.Invoke(id, this);
            //GameEventSystem.Call(GameEventType.Enemy_Death, id, this);

            healthBar.Hide();
            RemoveAndDestroy();
        }
    }

    public void EnableColliders(bool enabled)
    {
        if (boxCollider == null)
            boxCollider = GetComponent<BoxCollider>();
        boxCollider.enabled = enabled;
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(Constants.PLAYTERTAG))
        {
            IDestroyable destroyable = other.GetComponent<IDestroyable>();
            destroyable.TakeDamage(destroyable.CurrentHealth);
        }
    }

    public void RemoveAndDestroy()
    {
        GameController gameController = GameObject.FindObjectOfType<GameController>();
        if (gameController != null)
        {
            gameController.RemoveEnemy(this);
        }
        gameObject.SetActive(false);
    }
}
