using UnityEngine;

public abstract class Ship : MonoBehaviour, IDamagable
{
    public delegate void HealthChanged(float currentHealth, float maxHealth);
    public event HealthChanged OnHealthChanged;

    public int Id;
    public int Level { get; set; }
    public bool IsAlive { get; set; }
    public float Damage { get; set; }
    public float FireRate { get; set; }
    public float Speed { get; set; }
    public float CurrentHealth { get; set; }
    public float MaxHealth { get; set; }

    protected Animator animator;

    protected bool HasShield;

    [SerializeField] protected GameObject ShieldEffect;



    public virtual void OnDestroy() { }
    public virtual void OnDisable() { }
    public virtual void OnEnable() { }
    public virtual void Awake() { }
    public virtual void Start() { }
    public virtual void OnTriggerEnter(Collider other) { }
    public virtual void Update() { }
    public virtual void SetStats(int level) { }

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
    public virtual void Hit() { }

    public virtual void Death() { }
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

    public void SetCurrentHealth(float value)
    {
        CurrentHealth = value;
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }

    public float GetHealthPresentage()
    {
        return (CurrentHealth / MaxHealth) * 100;
    }

}