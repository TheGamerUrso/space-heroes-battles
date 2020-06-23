using System;
using TheGamerUrso.PoolSystem;
using UnityEngine;

public class BaseEnemy : Ship, IDamagable
{

    [Header("Enemy Config")]
    [SerializeField]protected bool AutoEnableWeapon;
    [SerializeField]protected BaseEnemyAI baseEnemyAI;
    [SerializeField]protected WeaponScript[] Weapons;
    [SerializeField]protected float delayAttak = 3;
    protected BoxCollider boxCollider;
    protected bool CanAttack;
    protected int currentWeaponActive;
    protected float takeDamageDelay;
    protected bool EnableShield;
    protected WeaponScript weaponScript;
    protected EnemyHealthWidget healthBar;


    [SerializeField] private HealthBarSettings HealthBarSettings;
    private BaseGameMode baseGameMode;



    [HideInInspector] public EnemyElement enemyElement;
    public int m_ValueOfEnemy;
    public float DeathDelay;

    #region Properties

    public EnemyHealthWidget HealthBar
    {
        get
        {
            return healthBar;
        }
    }

    public float DelayAttack
    {
        get { return delayAttak; }
    }

    #endregion
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
        if (baseGameMode == null)
        {
            baseGameMode = GameObject.FindObjectOfType<BaseGameMode>();
        }

        if (baseGameMode != null)
        {
            baseGameMode.UnregisterEnemy(this);
        }
    }

    private void OnDestroy()
    {
        if (baseGameMode == null)
        {
            baseGameMode = GameObject.FindObjectOfType<BaseGameMode>();
        }

        if (baseGameMode != null)
        {
            baseGameMode.UnregisterEnemy(this);
        }
    }

    public override void OnAwake()
    {
        SetStats(1);
        HasShield = false;
        boxCollider = GetComponent<BoxCollider>();
        animator = GetComponentInChildren<Animator>();

        if (baseGameMode == null)
        {
            baseGameMode = GameObject.FindObjectOfType<BaseGameMode>();
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

        if (baseGameMode != null)
        {
            baseGameMode.RegisterEnemy(this);
        }

    }

    public void Leave()
    {
        Events.EnemyEscaped?.Invoke(gameObject.name, this);
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
       Events.EnemyGotHit?.Invoke(gameObject.name, this);
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
            Events.EnemyDied?.Invoke(gameObject.name, this);
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
