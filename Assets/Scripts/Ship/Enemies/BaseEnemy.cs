using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnemyInformationAfterDeath : EventArgs
{
    public EnemyElement enemyElement;
    public int EnemyAward;
}

public class BaseEnemy : Ship, IDestroyable
{
    public bool Alive;

    [Header("Enemy Config")]
    public SimpleAI simpleAI;
    public Action<BaseEnemy> onEnemyDeath;
    public Action<object> OnEnemyHit;
    public Action<BaseEnemy> onEnemyEscape;
    [HideInInspector] public EnemyElement enemyElement;
    private EnemyHealthWidget healthBar;
    [SerializeField] private HealthBarSettings HealthBarSettings;
    public float DeathDelay;

    //IDestroyable Values
    public bool IsAlive
    {
        get { return Alive; }
        set { Alive = value; }
    }

    public int m_ValueOfEnemy;

    public PoolGameObjectType[] DropItems;

    protected WeaponScript weaponScript;
    protected float takeDamageDelay;
    protected bool EnableShield;
    private void OnDisable()
    {
        onEnemyDeath = null;
        onEnemyEscape = null;
    }
    private void OnEnable()
    {
        Alive = true;
    }

    public virtual void SetEnemyStats(int level, Action<BaseEnemy> OnEnemyDeath, Action<BaseEnemy> OnEnemyEscape)
    {
        GetLevelSystem().SetLevel(level);

        GetShipStatsSystem().SetStats(levelSystem);

        // int randomNum = UnityEngine.Random.Range(0, 100);

        //if (Level % 2 == 0 && randomNum >= 75)
        // {
        //     EnableShield = true;
        //}
        ShieldModuleInstalled = false;
        //if (EnableShield)
        // {
        //     int hasShield = UnityEngine.Random.Range(0, 100);
        //     if (hasShield <= 100)
        //      {
        //          ShieldModuleInstalled = true;
        //      }
        //      else if (hasShield > 100)
        //      {
        //          ShieldModuleInstalled = false;
        //      }
        //  }



        if (weaponScript)
        {
            weaponScript.SetShip(this);
            weaponScript.SetShipStatsSystem(GetShipStatsSystem());
        }

        this.onEnemyDeath = OnEnemyDeath;
        this.onEnemyEscape = OnEnemyEscape;
    }

    public override void InitReferences()
    {
        //shipStatsSystem = new ShipStatsSystem();
        //levelSystem = new LevelSystem();
        shipStatsSystem.SetStats(levelSystem);
        ShieldModuleInstalled = false;
    }

    public override void ShipStartSetUp()
    {
        if (healthBar != null)
        {
            healthBar.GetComponent<BaseHealthWidget>();
        }

        simpleAI = GetComponent<SimpleAI>();
        ShieldEffect.SetActive(ShieldModuleInstalled);

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

    public virtual void Enter() { }
    public void Leave()
    {
        if (onEnemyEscape != null) onEnemyEscape(this);
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

            if (ShieldModuleInstalled == true)
            {
                ShieldModuleInstalled = false;
                ShieldEffect.SetActive(ShieldModuleInstalled);
            }
            else if (ShieldModuleInstalled == false)
            {
                shipStatsSystem.CurrentHealth -= dmg;

                if (shipStatsSystem.CurrentHealth < 1)
                {
                    animator.SetBool("Death", true);
                    Death();
                }
            }

        }

        Hit();
    }

    public virtual void Hit()
    {

        if (OnEnemyHit != null)
        {
            OnEnemyHit(this);
        }
    }

    public virtual void Update()
    {
        if (takeDamageDelay >= 0)
        {
            takeDamageDelay -= Time.deltaTime;
        }
    }

    public override void Death()
    {
        if (Alive)
        {
            Alive = false;

            GameObject explostion = PoolManager.Instance.GetObjectFromPool(PoolGameObjectType.ShipExplosion);
            explostion.transform.position = transform.position;

            if (onEnemyDeath != null)
                onEnemyDeath(this);


            healthBar.Hide();
            gameObject.SetActive(false);

        }

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(Constants.PLAYTERTAG))
        {
            IDestroyable destroyable = other.GetComponent<IDestroyable>();
            destroyable.TakeDamage(destroyable.CurrentHealth);
        }
    }
}
