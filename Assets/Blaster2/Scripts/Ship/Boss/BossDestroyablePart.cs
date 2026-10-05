using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDestroyablePart : MonoBehaviour
{
    public HealthComponent healthComponent;

    public float baseHealth;
    [SerializeField] protected BossEnemy baseBossEnemy;
    [SerializeField] protected GameObject fireEffect;
    [SerializeField] protected GameObject prepareToAttack;
    [SerializeField] protected Animator animator;
    [Range(.1f, 1)]
    protected float takeDamageDelay;

    [Header("Combat Settings")]
    [SerializeField] private float punchDamage = 7.5f; // Damage dealt to the player
    private bool canDealDamage = false;
    private float lastHitFrame = -1f;
    private bool HasShield = false;
    [SerializeField] protected GameObject shieldEffect;

    private void Start()
    {
        fireEffect.SetActive(false);
        Setup(baseHealth);

        bool hasRandomShield = UnityEngine.Random.value <= 0.2f;

        if (healthComponent != null)
        {
            // Pass max health and the shield flag into the new Setup overload
            healthComponent.Setup(baseHealth, hasRandomShield);

            // Turn on/off the visual effect based on whether the component has a shield
            if (shieldEffect != null)
            {
                shieldEffect.SetActive(healthComponent.HasShield);
            }

            healthComponent.OnHealthChanged += (current, max) =>
            {
                // If shield breaks or health drops, update visuals
                if (shieldEffect != null && !healthComponent.HasShield)
                {
                    shieldEffect.SetActive(false);
                }

                if (current <= 0)
                {
                    fireEffect.SetActive(true);
                }
            };
        }
    }
    private void OnDestroy()
    {
        healthComponent.OnHealthChanged = null;
        healthComponent.OnHealthChanged = null;
    }

    public void Setup(float baseHealth)
    {
        var rand = UnityEngine.Random.value;
        HasShield = rand <= .2f;
    }

    public void PrepareAttack()
    {
        if (healthComponent.IsAlive)
            prepareToAttack.SetActive(true);
    }

    public void Attack()
    {
        canDealDamage = true;
        prepareToAttack.SetActive(false);
        if (healthComponent.IsAlive)
            animator.SetTrigger("Attack");
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
