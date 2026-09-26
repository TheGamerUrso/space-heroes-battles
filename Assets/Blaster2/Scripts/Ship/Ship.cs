using System;
using UnityEngine;

[Serializable]
public struct Stats 
{
    public int Level;
    public float Damage;
    public float FireRate;
    public float Speed;
    public float Health;
}

public abstract class Ship : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    [SerializeField] protected AudioSource audioSource;
    public HealthComponent healthComponent;
    public WeaponController weaponController;
    public BaseMovementController movementController;

    [Header("STATS")]
    public Stats stats;
    public bool HasShield;
    public GameObject ShieldEffect;
    public virtual void SetStats(int level, float baseHealth, float baseSpeed, float baseDamage, float baseFireRate)
    {
        stats.Level = Mathf.Clamp(level, 1, 10);

        float healthGrowthRate = 0.25f;
        float damageGrowthRate = 0.18f;

        stats.Health = baseHealth * (1f + (healthGrowthRate * (stats.Level - 1)));

        stats.Speed = baseSpeed;

        stats.Damage = baseDamage * (1f + (damageGrowthRate * (stats.Level - 1)));
        stats.FireRate = baseFireRate;

        healthComponent.Setup(stats.Health, false);
        weaponController.Setup(this, stats.Damage, stats.FireRate);
        movementController.SetSpeed(stats.Speed);
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Death() { }
    public virtual void Hit() { }

    public virtual void ActiveShield()
    {
        if (HasShield)
            return;

        HasShield = true;
        if (ShieldEffect != null) ShieldEffect.SetActive(HasShield);
    }
    public virtual void DeactivateShield()
    {
        if (!HasShield)
            return;

        HasShield = false;
        if (ShieldEffect != null) ShieldEffect.SetActive(HasShield);
    }
}