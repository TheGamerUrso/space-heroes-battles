using System;
using UnityEngine;

public class BaseEnemy : Ship, IDamagable
{
    public event Action<float, float> OnHealthChanged;

    public int Id;
    public Enemy_SO EnemyData;
    public bool IsAlive { get; set; }

    public int Level;
    public float Damage;
    public float FireRate;
    public float Speed;
    public float currentHealth;
    public float maxHealth;
    public float CurrentHealth { get { return currentHealth; } }
    public float MaxHealth { get { return maxHealth; } }

    protected BaseEnemyAI baseEnemyAI;
    protected bool AutoEnableWeapon;

    [SerializeField] protected WeaponScript[] Weapons;
    [SerializeField] protected float delayAttak = 3;
    protected BoxCollider boxCollider;
    protected bool CanAttack;
    protected int currentWeaponActive;
    protected float takeDamageDelay;

    public EnemyHealthWidget HealthBar { get; set; }

    private BaseGameMode baseGameMode;

    [HideInInspector] public EnemyElement enemyElement;

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

    public override void Update()
    {
        if (takeDamageDelay >= 0)
        {
            takeDamageDelay -= Time.deltaTime;
        }
    }

    public void Leave()
    {
        Events.EnemyEscaped?.Invoke(gameObject.name, this);
    }

    public virtual void TakeDamage(float dmg)
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
                currentHealth -= dmg;

                OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

                if (currentHealth < 1)
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

    public virtual void Death()
    {
        if (IsAlive)
        {
            IsAlive = false;
            var explostion = PoolManager.Instance.GetObjectFromPool(EnemyData.ExplostionEffect);
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

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(Constants.PLAYTERTAG))
        {
            var destroyable = other.GetComponent<IDamagable>();
            destroyable.TakeDamage(destroyable.MaxHealth / 2);
            TakeDamage(destroyable.CurrentHealth);
        }
    }

    public override void SetStats(int level)
    {
        Level = level;

        maxHealth = Level * EnemyData.baseHealth;

        currentHealth = MaxHealth;

        Speed = EnemyData.baseSpeed;

        Damage = Level * EnemyData.baseDamage;

        FireRate = EnemyData.baseFireRate;

        HasShield = false;

        for (int weaponIndex = 0; weaponIndex < Weapons.Length; weaponIndex++)
        {
            Weapons[weaponIndex].Damage = Damage;
            Weapons[weaponIndex].FireRate = FireRate;
        }

    }

    public float GetHealthPresentage()
    {
        return (CurrentHealth / MaxHealth) * 100;
    }

    public virtual void Heal(float ammount)
    {

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

    public void SetHealth(float health)
    {
        currentHealth = health;
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }

}
