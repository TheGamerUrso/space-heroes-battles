using System;
using UnityEngine;

public abstract class Ship : MonoBehaviour
{
    public Action<float, float> OnHealthChanged;
    protected Animator animator;
    [SerializeField] protected AudioSource audioSource;
    #region Health

    public bool IsAlive { get; set; }
    public virtual BaseHealthWidget HealthBar { get; set; }
    #endregion
    #region Shield
    protected bool HasShield;
    [SerializeField] protected GameObject ShieldEffect;

    #endregion
    #region Weapons
    public int CurrentWeapnType { get; set; }
    public bool CanFire { get; protected set; }
    #endregion Weapons

    #region Stats
    public float MaxHealth { get; protected set; }
    public float CurrentHealth { get; protected set; }
    #endregion

    #region Methods
    public virtual void OnDestroy() { }
    public virtual void OnDisable() { }
    public virtual void OnEnable() { }
    public virtual void Awake() { }
    public virtual void Start() { }
    public virtual void Update() { }
    public virtual void SetStats(int level) { }

    public virtual void InstallShieldModule()
    {
        if (HasShield)
        {
            return;
        }

        HasShield = true;
    }
    public virtual float GetHealthPresentage()
    {
        return (CurrentHealth / MaxHealth);
    }

    public abstract void Heal(float ammount);
    public bool HasShieldModule()
    {
        return HasShield;
    }
    public abstract void SwitchWeapon(int id, bool solo = false);

    public virtual void SetHealth(float health)
    {
        CurrentHealth = health;
        this.OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }

    public virtual void EnableFire()
    {
        CanFire = true;
    }
    public virtual void DisableFire()
    {
        CanFire = false;
    }
    public abstract void EnterLevel();
    public abstract void ExitLevel();

    public virtual void Death() { }

    public virtual void InstallShield() { }

    public bool IsDead()
    {
        return CurrentHealth <= 0;
    }
    public virtual void TakeDamage(float dmg) { }

    public virtual void Hit() { }
    #endregion
}