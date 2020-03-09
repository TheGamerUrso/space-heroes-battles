using System;
using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.PoolSystem;
using UnityEngine;

public class BaseEnemy : Ship, IDestroyable
{
    private GameEventSystem gameEventSystem;
    protected string id;

    [Header("Enemy Config")]
    public BaseEnemyAI baseEnemyAI;
    protected BoxCollider boxCollider;

    //public Action<BaseEnemy> onEnemyDeath;
    //public Action<object> OnEnemyHit;
    //public Action<BaseEnemy> onEnemyEscape;

    public      bool Alive;
    protected   bool CanAttack;
    [SerializeField] 
    protected   GameObject[] Weapons;
    [SerializeField]
    protected   float delayAttak = 3;
    public      float DeathDelay;
    protected   int currentWeaponActive;

    private EnemyHealthWidget healthBar;

    [HideInInspector] public EnemyElement enemyElement;
   
    [SerializeField] private HealthBarSettings HealthBarSettings;


    //IDestroyable Values
    public bool IsDestroyed{ get; set; } = false;
    public int m_ValueOfEnemy;

    public PoolGameObjectType[] DropItems;

    protected WeaponScript weaponScript;
    protected float takeDamageDelay;
    protected bool EnableShield;

    private void OnDisable()
    {

    }

    private void OnEnable()
    {
        Alive = true;
    }

    public virtual void SetEnemyStats(int level)
    {
        GetLevelSystem().SetLevel(level);
        GetShipStatsSystem().SetStats(levelSystem);
        bShieldModuleInstalled = false;

        if (weaponScript)
        {
            weaponScript.SetShip(this);
            weaponScript.SetShipStatsSystem(GetShipStatsSystem());
        }
    }

    public override void InitReferences()
    {
        shipStatsSystem.SetStats(levelSystem);
        bShieldModuleInstalled = false;
        boxCollider = GetComponent<BoxCollider>();
    }

    public override void ShipStartSetUp()
    {
        if (healthBar != null)
        {
            healthBar.GetComponent<BaseHealthWidget>();
        }

        baseEnemyAI = GetComponent<SimpleAI>();
        ShieldEffect.SetActive(bShieldModuleInstalled);

        weaponScript = GetComponentInChildren<WeaponScript>(true);
        if (weaponScript != null)
        {
            weaponScript.SetShipStatsSystem(shipStatsSystem);
        }

        if (HealthBarSettings != null)
        {
            GameObject initializedHealthWidget = Instantiate(HealthBarSettings.HealthBarPrefab, transform, false);
            healthBar = (EnemyHealthWidget)initializedHealthWidget.GetComponent<BaseHealthWidget>();
            healthBar.Initiallize(this);
        }
    }
    public virtual void Attack(){}

    public virtual void Enter() {}

    public void Leave()
    {
       GameEventSystem.Call(GameEventType.Enemy_Escape,id,this);
    }

    public virtual void Heal(float ammount)
    {
        shipStatsSystem.CurrentHealth += ammount;

        if (shipStatsSystem.CurrentHealth > MaxHealth)
        {
            shipStatsSystem.CurrentHealth = MaxHealth;
        }

        shipStatsSystem.CurrentHealth = Mathf.Clamp(shipStatsSystem.CurrentHealth, 0, MaxHealth);
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
                shipStatsSystem.CurrentHealth -= dmg;

                if (shipStatsSystem.CurrentHealth < 1)
                {
                    Death();
                }
            }

        }

        Hit();
    }

    public virtual void Hit()
    {
        GameEventSystem.Call(GameEventType.Enemy_Hit, id,this);
    }

    public virtual void Update()
    {
        if (takeDamageDelay >= 0)
        {
            takeDamageDelay -= Time.deltaTime;
        }
        Tick();
    }

    public virtual void Tick(){}

 

    public override void Death()
    {
        if (Alive)
        {
            Alive = false;

            GameObject explostion = PoolManager.Instance.GetObjectFromPool(PoolGameObjectType.ShipExplosion);
            explostion.transform.position = transform.position;

            GameEventSystem.Call(GameEventType.Enemy_Death, id,this);


            healthBar.Hide();
            gameObject.SetActive(false);
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
}
