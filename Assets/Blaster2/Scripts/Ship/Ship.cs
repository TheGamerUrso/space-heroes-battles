using System;
using TheGamerUrso.Core;
using UnityEngine;

public abstract class Ship : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    [SerializeField] protected AudioSource audioSource;
    public HealthComponent healthComponent;
    public WeaponController weaponController;
    public BaseMovementController movementController;

    [Header("STATS")]
    public int Level;
    public float Damage;
    public float FireRate;
    public float Speed;
    public float Health;

    public virtual void SetStats(int level, float baseHealth, float baseSpeed, float baseDamage, float baseFireRate)
    {
        Level = Mathf.Clamp(level, 1, 10);

        float healthGrowthRate = 0.25f;
        float damageGrowthRate = 0.18f;

        Health = baseHealth * (1f + (healthGrowthRate * (Level - 1)));

        Speed = baseSpeed;

        Damage = baseDamage * (1f + (damageGrowthRate * (Level - 1)));
        FireRate = baseFireRate;

        healthComponent.Setup(Health, false);
        weaponController.Setup(this,Damage, FireRate);
        movementController.SetSpeed(Speed);
    }
}