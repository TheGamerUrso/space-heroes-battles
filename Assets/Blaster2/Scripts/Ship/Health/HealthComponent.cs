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

    protected float invisibilityTimer;

    private void Update()
    {
        if (invisibilityTimer >= 0)
        {
            invisibilityTimer -= Time.deltaTime;
        }  
    }

    public void Setup(Ship ship)
    {
        maxHealth = ship.shipData.Health;
        currentHealth = maxHealth;
        IsAlive = true;
    }

    public virtual void TakeDamage(float dmg,bool IgnoreShield = false)
    {
        if (!IsAlive) return;

        audioSource.PlayOneShot(hitSFX);

        bool IsShieldActive = IgnoreShield ? false : ship.shipData.HasShield;

        if (invisibilityTimer <= 0)
        {
            invisibilityTimer = .25f;
            if (!IsShieldActive)
            {
                currentHealth -= dmg;
            }
            if (CurrentHealth < 1)
            {
                IsAlive = false;
            }

        }
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
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
    //=================================================================================
    public void tempGodMode()
    {
        invisibilityTimer = 1;
    }
}
