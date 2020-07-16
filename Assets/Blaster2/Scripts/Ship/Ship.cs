using TheGamerUrso.PoolSystem;
using UnityEngine;

public abstract class Ship : MonoBehaviour
{
    public delegate void HealthChanged(float currentHealth, float maxHealth);
    public event HealthChanged OnHealthChanged;
    public ShipStats shipStats;

    public int Id;

    protected bool Alive;

    public bool IsAlive
    {
        get
        {
            return Alive;
        }
        private set
        {
            Alive = value;
        }
    }


    /**
   * Attributes
   */
    [Min(12.5f)]
    [HideInInspector] public float Damage;
    [Min(.2f)]
    [Range(.2f, 10)]
    [HideInInspector] public float FireRate;
    [Min(0)]
    [HideInInspector] public float Speed;
    [Min(0)]
    public float currentHealth;
    [Min(25)]
    [HideInInspector] protected float maxHealth;
    public float CurrentHealth
    {
        get
        {
            return currentHealth;
        }
        set
        {
            currentHealth = value;
            OnHealthChanged?.Invoke(currentHealth, MaxHealth);
        }
    }

    public float MaxHealth
    {
        get
        {
            return maxHealth;
        }
        set
        {
            maxHealth = value;
        }
    }

    public float HealthPresentage
    {
        get
        {
            return (CurrentHealth / MaxHealth) * 100;
        }
    }
    public int Level { get; set; }
    public int MaxLevel { get; set; }

    protected Animator animator;

    protected bool HasShield;

    [Header("Effects")]
    [SerializeField] protected GameObject ShieldEffect;
    [SerializeField] protected PoolGameObjectType ExplostionEffect;


    private void OnDestroy()
    {
        OnCleanUp();
    }

    public virtual void OnCleanUp() { }

    private void OnEnable()
    {
        Enter();
    }

    public virtual void Enter() { }

    private void Awake()
    {
        OnAwake();

    }
    private void Start()
    {
        ShipSetup();
    }
    public abstract void ShipSetup();
    public abstract void OnAwake();
    public abstract void Death();

    public virtual void InstallShieldModule()
    {
        if (HasShield)
        {
            return;
        }

        HasShield = true;
    }

    public bool HasShieldModule()
    {
        return HasShield;
    }

    public virtual void SetStats(int level)
    {
        MaxLevel = 20;
        Level = level;

        maxHealth = Level * shipStats.baseHealth;

        CurrentHealth = MaxHealth;

        Speed = shipStats.baseSpeed;

        Damage = Level * shipStats.baseDamage;

        FireRate = shipStats.baseFireRate;
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
    }
}