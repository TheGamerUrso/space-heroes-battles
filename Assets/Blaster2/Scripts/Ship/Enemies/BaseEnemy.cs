using UnityEngine;

public class BaseEnemy : Ship
{
    public Enemy_SO EnemyData;

    [SerializeField] protected bool AutoEnableWeapon;
    [SerializeField] protected BaseEnemyAI baseEnemyAI;
    [SerializeField] protected WeaponScript[] Weapons;
    [SerializeField] protected float delayAttak = 3;
    protected BoxCollider boxCollider;
    protected bool CanAttack;
    protected int currentWeaponActive;
    protected float takeDamageDelay;
    protected bool EnableShield;
    protected WeaponScript weaponScript;
    public EnemyHealthWidget HealthBar { get; set; }

    private BaseGameMode baseGameMode;

    [HideInInspector] public EnemyElement enemyElement;

    public float DeathDelay;

    public float DelayAttack
    {
        get { return delayAttak; }
    }


    public override void OnEnable()
    {
        IsAlive = true;

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

    public override void OnDisable()
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

    public override void Awake()
    {
        HasShield = false;
        boxCollider = GetComponent<BoxCollider>();
        animator = GetComponentInChildren<Animator>();

        if (baseGameMode == null)
        {
            baseGameMode = GameObject.FindObjectOfType<BaseGameMode>();
        }
    }

    public override void Start()
    {
        SetStats(1);

        if (HealthBar != null)
        {
            HealthBar.GetComponent<BaseHealthWidget>();
        }

        if (EnemyData.HealthBarSettings != null)
        {
            GameObject initializedHealthWidget = Instantiate(EnemyData.HealthBarSettings.HealthBarPrefab, transform, false);
            HealthBar = (EnemyHealthWidget)initializedHealthWidget.GetComponent<BaseHealthWidget>();
            HealthBar.Setup(this, false);
        }

        baseEnemyAI = GetComponent<SimpleAI>();
        ShieldEffect.SetActive(HasShield);

        SetStats(Level);
    }

    public virtual void Attack() { }

    public void Leave()
    {
        Events.EnemyEscaped?.Invoke(gameObject.name, this);
    }

    public override void TakeDamage(float dmg)
    {
        if (IsAlive == false)
        {
            return;
        }

        AudioManager.PlaySound(EnemyData.hitSFX);

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

    public override void Hit()
    {
        Events.EnemyGotHit?.Invoke(gameObject.name, this);
    }

    public override void Update()
    {
        if (takeDamageDelay >= 0)
        {
            takeDamageDelay -= Time.deltaTime;
        }
    }

    public override void Death()
    {
        if (IsAlive)
        {
            IsAlive = false;
            GameObject explostion = PoolManager.Instance.GetObjectFromPool(PoolGameObjectType.ShipExplosion);
            explostion.transform.position = transform.position;
            Events.EnemyDied?.Invoke(gameObject.name, this);
            HealthBar.Hide();
            gameObject.SetActive(false);
        }
    }

    public void EnableColliders(bool enabled)
    {
        if (boxCollider == null)
            boxCollider = GetComponent<BoxCollider>();
        boxCollider.enabled = enabled;
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(Constants.PLAYTERTAG))
        {
            IDamagable destroyable = other.GetComponent<IDamagable>();
            destroyable.TakeDamage(destroyable.MaxHealth / 2);
            TakeDamage(CurrentHealth);
        }
    }

    public override void SetStats(int level)
    {
        Level = level;

        MaxHealth = Level * EnemyData.baseHealth;

        CurrentHealth = MaxHealth;

        Speed = EnemyData.baseSpeed;

        Damage = Level * EnemyData.baseDamage;

        FireRate = EnemyData.baseFireRate;

        HasShield = false;

        if (weaponScript)
        {
            weaponScript.SetShip(this);
        }

    }

}
