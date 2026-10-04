using System;
using TheGamerUrso.Core;
using UnityEditor.MPE;
using UnityEngine;

public class HealthComponent : MonoBehaviour , IDamagable
{
    [SerializeField] public Ship ship;
    public Action<float, float> OnHealthChanged;
    public bool IsAlive { get; protected set; }

    [Header("Health")]
    [SerializeField] protected float currentHealth;
    [SerializeField] protected float maxHealth;

    [Header("Shield")]
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected AudioClip hitSFX;
    [SerializeField] protected BoxCollider boxCollider;
    private float lastHitFrame = -1f;
    public float lastHitTime = 0;
    public float hitCooldown = 0;

    public void Setup(Ship ship)
    {
        maxHealth = ship.shipData.Health;
        currentHealth = maxHealth;
        IsAlive = true;
    }

    public virtual void TakeDamage(float dmg, bool IgnoreShield = false)
    {
        if (!IsAlive) return;

        audioSource.PlayOneShot(hitSFX);

        bool IsShieldActive = IgnoreShield ? false : ship.shipData.HasShield;
        if (IsShieldActive)
        {
            ship.DeactivateShield();

            return;
        }
        // 1. Check if this damage is part of a simultaneous multi-bullet spread hit (same frame)
        bool isSameFrameBurst = (Time.frameCount == lastHitFrame);
        // 2. If it's NOT the same frame, enforce your rapid-fire hit cooldown
        if (!isSameFrameBurst && (Time.time - lastHitTime < hitCooldown))
        {
            return; // Block rapid-fire spam, but allow simultaneous multi-bullets!
        }
        // 3. Apply Damage
        currentHealth -= dmg;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        // 4. Play audio ONLY once per hit event (prevents audio spam from multi-bullets)
        if (!isSameFrameBurst && hitSFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSFX);
        }

        // 5. Update tracking variables
        lastHitTime = Time.time;
        lastHitFrame = Time.frameCount;

        CheckDeath();
    }
    //=================================================================================
    public void CheckDeath()
    {
        if (CurrentHealth < 1)
        {
            IsAlive = false;
        }
    }
    //=================================================================================
    public virtual void Heal(float amount)
    {
        currentHealth += amount;
        if (CurrentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        this.OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }
    //=================================================================================
    public virtual float GetHealthPresentage()
    {
        return (CurrentHealth / maxHealth);
    }
    //=================================================================================
    public void SetDamagable(bool enabled)
    {
        if (boxCollider == null)
            boxCollider = GetComponent<BoxCollider>();
        boxCollider.enabled = enabled;
    }   
}
