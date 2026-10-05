using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDestroyablePart : MonoBehaviour, IDamagable
{
    public float baseHealth;
    [SerializeField] protected BossEnemy baseBossEnemy;
    [SerializeField] protected GameObject fireEffect;
    [SerializeField] protected GameObject prepareToAttack;
    [SerializeField] protected BoxCollider boxCollider;
    [SerializeField] protected Animator animator;
    public AudioClip hitSFX;
    [Range(.1f, 1)]
    protected float takeDamageDelay;

    [Header("Combat Settings")]
    [SerializeField] private float punchDamage = 7.5f; // Damage dealt to the player
    private bool canDealDamage = false;

    public Action<float, float> OnHealthChanged;
    public bool IsAlive { get; protected set; }

    [Header("Health")]
    [SerializeField] protected float currentHealth;
    [SerializeField] protected float maxHealth;

    [Header("Shield")]
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    [SerializeField] protected AudioSource audioSource;

    private float lastHitFrame = -1f;
    public float lastHitTime = 0;
    public float hitCooldown = 0;
    private bool HasShield = false;
    [SerializeField] protected GameObject ShieldtEffect;

    private void Start()
    {
        fireEffect.SetActive(false);
        Setup(baseHealth);
    }
    private void OnDestroy()
    {
        OnHealthChanged = null;
        OnHealthChanged = null;
    }

    public void Setup(float baseHealth)
    {
        maxHealth = baseHealth;
        currentHealth = maxHealth;
        IsAlive = true;
        var rand = UnityEngine.Random.value;
        HasShield = rand <= .2f;
        ShieldtEffect.SetActive(HasShield);

    }


    public void PrepareAttack()
    {
        if (IsAlive)
            prepareToAttack.SetActive(true);
    }

    public void Attack()
    {
        canDealDamage = true;
        prepareToAttack.SetActive(false);
        if (IsAlive)
            animator.SetTrigger("Attack");
    }
    public virtual void TakeDamage(float dmg, bool IgnoreShield = false)
    {
        if (!IsAlive) return;

        audioSource.PlayOneShot(hitSFX);

        bool IsShieldActive = IgnoreShield ? false : HasShield;
        if (IsShieldActive)
        {
            ShieldtEffect.SetActive(false);

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

        if (currentHealth <= 0)
        {
            fireEffect.SetActive(true);
        }

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
    public Animator GetAnimator()
    {
        return animator;
    }
    public void OnTriggerEnter(Collider other)
    {
        // Only deal damage if this specific punch is currently executing an attack
        if (!canDealDamage) return;

        // Check if we hit the player (adjust tag or component check to match your project)
        if (other.CompareTag("Player"))
        {
            // Try to find the player's health component and deal damage
            HealthComponent playerHealth = other.GetComponent<HealthComponent>();
            if (playerHealth != null)
            {
                // Deal damage (passing shipOwner as the dealer/attacker if required by your health system)
                playerHealth.TakeDamage(punchDamage); // Adjust method name based on your HealthComponent API
            }

            // Disable damage so a single punch doesn't multi-hit the player frame-by-frame
            canDealDamage = false;
            CancelAttack();
        }
    }
    public void CancelAttack()
    {
        // Instantly cut off the punch animation and force transition back to Idle
        if (animator != null)
        {
            animator.SetTrigger("CancelAttack");
        }

        // Reset your damage or warning flags
        prepareToAttack.SetActive(false);
        canDealDamage = false;
    }
}
