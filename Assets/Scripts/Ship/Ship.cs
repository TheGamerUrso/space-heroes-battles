using TheGamerUrso.PoolSystem;
using UnityEngine;

public abstract class Ship : MonoBehaviour, IDestroyable
{
    protected bool Alive;
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
    /**
   * Attributes
   */
    #region Attributes
    public delegate void HealthChanged(float currentHealth, float maxHealth);
    public event HealthChanged OnHealthChanged;
    public ShipStats shipStats;

    [Range(.2f, 10)]

    [Min(.2f)]
    public float FireRate;

    [Min(0)]
    public float Speed;

    [Min(25)]
    protected float maxHealth;

    [Min(0)]
    public float currentHealth;

    [Min(12.5f)]
    public float Damage;

    public float CurrentHealth
    {
        get
        {
            return currentHealth;
        }
        set
        {
            currentHealth = value;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
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
    #endregion Attributes

    /**
     * Level System
     */

    #region Level System
    public delegate void XpChanged(int level, float xp, float xpToLevel);
    public event XpChanged OnXpChanged;

    public delegate void LevelUp(int level);
    public event LevelUp OnLevelUp;

    public float xpToLevel;

    public float xp;
    public float XP
    {
        get
        {
            return xp;
        }

        set
        {
            xp = value;
            OnXpChanged?.Invoke(level, xp, xpToLevel);
        }
    }

    [Min(20)]
    public int maxLevel;
    public int MaxLevel
    {
        get
        {
            return maxLevel;
        }
        set
        {
            maxLevel = value;
        }
    }

    [Range(1, 20)]
    public int level;
    public int Level
    {
        get
        {
            return level;
        }
        set
        {
            level = value;
        }
    }

    public float XPPresentage
    {
        get
        {
            return (float)xp / xpToLevel;
        }
    }
    #endregion Level System

    protected Animator animator;

    protected bool HasShield;

    [Header("Effects")]
    [SerializeField] protected GameObject ShieldEffect;
    [SerializeField] protected PoolGameObjectType ExplostionEffect;

    private void OnEnable()
    {
        Enter();
    }

    public virtual void Enter()
    {

    }
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
        Level = level;
        var multiplier = level / 10;

        MaxHealth = Level * shipStats.baseHealth;

        CurrentHealth = MaxHealth;

        Speed = shipStats.baseSpeed;

        Damage = Level * shipStats.baseDamage;

        FireRate = shipStats.baseFireRate;

        Damage = Level * shipStats.baseDamage;
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

    public void AddXP(float ammount)
    {
        if (level < MaxLevel)
        {
            XP += ammount;
            if (xp >= xpToLevel)
            {
                Level++;
                XP -= xpToLevel;
                xpToLevel = (level / 10 + level % 10) * 100 * Mathf.Pow(10, level / 10);
                OnLevelUp?.Invoke(level);
                GameEventSystem.Call(PlayerEventType.Player_LevelUp);
                SetStats(level);
            }
        }
        else
        {
            level = MaxLevel;
            xp = 0;
        }
    }

    public void OnXPValueChanged(HealthChanged callback)
    {
        OnHealthChanged += callback;
    }

    public void OnHealthValueChanged(HealthChanged callback)
    {
        OnHealthChanged += callback;
    }

    public virtual void TakeDamage(float dmg)
    {
    }
}