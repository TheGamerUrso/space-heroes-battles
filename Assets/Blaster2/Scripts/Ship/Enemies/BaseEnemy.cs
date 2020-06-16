using System;
using TheGamerUrso.PoolSystem;
using UnityEngine;

public class BaseEnemy : Ship, IDamagable
{
    public Action<string, BaseEnemy> EnemyDied;
    public Action<string, BaseEnemy> EnemyGotHit;
    public Action<string, BaseEnemy> EnemyEscaped;

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

    public float DelayAttack
    {
        get { return delayAttak; }
    }

    [HideInInspector] public EnemyElement enemyElement;

    [SerializeField] private HealthBarSettings HealthBarSettings;

    protected EnemyHealthWidget healthBar;
    public EnemyHealthWidget HealthBar
    {
        get
        {
            return healthBar;
        }
    }



    public int m_ValueOfEnemy;

    protected float takeDamageDelay;
    protected bool EnableShield;

    protected WeaponScript weaponScript;
    [SerializeField] protected bool AutoEnableWeapon;


    private SpawnEnemies spawnEnemies;

    public void EnableWeaponById(int id, bool solo = false)
    {
        if (solo)
        {
            DisableAllWeapons();
        }

        Weapons[id].AutoAttack = true;
    }

    public void EnableAllWeapon()
    {
        if (Weapons.Length > 0)
        {
            for (int i = 0; i < Weapons.Length; i++)
            {
                Weapons[i].AutoAttack = true;
            }
        }
    }

    public void DisableAllWeapons()
    {
        if (Weapons.Length > 0)
        {
            for (int i = 0; i < Weapons.Length; i++)
            {
                Weapons[i].AutoAttack = false;
            }
        }
    }

    public virtual void SetEnemyStats(int level)
    {
        Level = level;
        SetStats(level);
        HasShield = false;

        if (weaponScript)
        {
            weaponScript.SetShip(this);
        }
    }

    private void OnDisable()
    {
        if (spawnEnemies == null)
        {
            spawnEnemies = GameObject.FindObjectOfType<SpawnEnemies>();
        }

        if (spawnEnemies != null)
        {
            spawnEnemies.UnregisterEnemy(this);
        }
    }

    private void OnDestroy()
    {
        if (spawnEnemies == null)
        {
            spawnEnemies = GameObject.FindObjectOfType<SpawnEnemies>();
        }

        if (spawnEnemies != null)
        {
            spawnEnemies.UnregisterEnemy(this);
        }
    }

    public override void OnAwake()
    {
        SetStats(1);
        HasShield = false;
        boxCollider = GetComponent<BoxCollider>();
        animator = GetComponentInChildren<Animator>();

        if (spawnEnemies == null)
        {
            spawnEnemies = GameObject.FindObjectOfType<SpawnEnemies>();
        }

    }

    public override void ShipSetup()
    {
        if (healthBar != null)
        {
            healthBar.GetComponent<BaseHealthWidget>();
        }

        if (HealthBarSettings != null)
        {
            GameObject initializedHealthWidget = Instantiate(HealthBarSettings.HealthBarPrefab, transform, false);
            healthBar = (EnemyHealthWidget)initializedHealthWidget.GetComponent<BaseHealthWidget>();
            healthBar.Setup(this, false);
        }

        baseEnemyAI = GetComponent<SimpleAI>();
        ShieldEffect.SetActive(HasShield);

        SetStats(Level);
    }


    public virtual void Attack() { }

    public override void Enter()
    {
        Alive = true;

        DisableAllWeapons();

        if (AutoEnableWeapon)
        {
            EnableAllWeapon();
        }

        if (spawnEnemies != null)
        {
            spawnEnemies.RegisterEnemy(this);
        }

    }

    public void Leave()
    {
        EnemyEscaped?.Invoke(gameObject.name, this);
    }

    public override void TakeDamage(float dmg)
    {
        if (Alive == false)
        {
            return;
        }


        AudioManager.PlaySound(shipStats.hitSFX);

        if (takeDamageDelay <= 0)
        {
            takeDamageDelay = .1f;

            if (HasShield == true)
            {
                HasShield = false;
                ShieldEffect.SetActive(HasShield);
            }
            else if (HasShield == false)
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
        EnemyGotHit?.Invoke(gameObject.name, this);
    }

    public virtual void Update()
    {
        if (takeDamageDelay >= 0)
        {
            takeDamageDelay -= Time.deltaTime;
        }
        OnUpdate();
    }

    public virtual void OnUpdate() { }

    public override void Death()
    {
        if (Alive)
        {
            Alive = false;
            GameObject explostion = PoolManager.Instance.GetObjectFromPool(PoolGameObjectType.ShipExplosion);
            explostion.transform.position = transform.position;
            EnemyDied?.Invoke(gameObject.name, this);
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
            IDamagable destroyable = other.GetComponent<IDamagable>();
            destroyable.TakeDamage(destroyable.MaxHealth / 2);
            TakeDamage(CurrentHealth);
        }
    }

}
