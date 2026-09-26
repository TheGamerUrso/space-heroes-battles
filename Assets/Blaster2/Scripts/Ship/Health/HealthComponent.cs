using System;
using TheGamerUrso.Core;
using UnityEditor.MPE;
using UnityEngine;

public class HealthComponent : MonoBehaviour , IDamagable
{
    public Ship ship;
    public Action<float, float> OnHealthChanged;
    public bool IsAlive { get; protected set; }

    [Header("Health")]
    [SerializeField] protected float currentHealth;
    [SerializeField] protected float maxHealth;

    [Header("Shield")]
    [SerializeField] protected GameObject ShieldEffect;
    public bool HasShield { get; protected set; }

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected AudioClip hitSFX;
    [SerializeField] protected BoxCollider boxCollider;


    protected float invisibilityTimer;
    protected int hitIndex;
    protected int numberOfHits;
    protected IEventService eventService;


    private void Start()
    {
        eventService = GameContext.Get<IEventService>();
    }

    private void Update()
    {
        if (invisibilityTimer >= 0)
        {
            invisibilityTimer -= Time.deltaTime;
        }
    }

    public void Setup(float baseHealth,bool startWithShield)
    {
        maxHealth = baseHealth;
        currentHealth = maxHealth;
        IsAlive = true;

        if (startWithShield)
            ship.ActiveShield();
        else
            ship.DeactivateShield();
    }
    public virtual void TakeDamage(float dmg)
    {
        if (IsAlive == false)
        {
            return;
        }

        audioSource.PlayOneShot(hitSFX);
        if (dmg >= maxHealth)
        {
            dmg = maxHealth - 1;
        }

        if (HasShield == true)
        {
            HasShield = false;
            ShieldEffect.SetActive(HasShield);
        }
        else if (HasShield == false)
        {
            if (invisibilityTimer <= 0)
            {
                invisibilityTimer = .25f;

                currentHealth -= dmg;

                OnHealthChanged?.Invoke(CurrentHealth, maxHealth);

                if (CurrentHealth < 1)
                {
                    if (IsAlive)
                    {
                        IsAlive = false;
                        ship.Death();
                    }
                }
            }
        }
        ship.Hit();
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
