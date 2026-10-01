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
    public abstract void SetStats(int level,
        float baseHealth,
        float baseSpeed,
        float baseDamage,
        float baseFireRate,
        float baseSuperDamage = 0,
        float baseSpecialCountdown = 0);

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Death() { }
    public virtual void Hit() { }

    public virtual void ActiveShield()
    {
        HasShield = true;
        if (ShieldEffect != null) ShieldEffect.SetActive(HasShield);
    }
    public virtual void DeactivateShield()
    {
        HasShield = false;
        if (ShieldEffect != null) ShieldEffect.SetActive(HasShield);
    }
}