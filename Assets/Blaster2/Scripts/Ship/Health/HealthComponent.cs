using System;
using TheGamerUrso.Core;
using UnityEditor.MPE;
using UnityEngine;

public class HealthComponent : MonoBehaviour , IDamagable
{
    public Ship ship;
    public Action<float, float> OnHealthChanged;
    public event Action<int, int> OnHit;
    public event Action OnDeath;

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
    [SerializeField] protected PoolGameObjectType explostionEffect;

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
            ActiveShield();
        else
            DeactivateShield();
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
                    Death();
                }
            }
        }
        Hit();
    }

    public virtual void Heal(float amount)
    {
        currentHealth += amount;

        if (CurrentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        this.OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

    public virtual void Hit()
    { 
        hitIndex++;
    }

    public virtual void Death()
    {
        if (IsAlive)
        {
            IsAlive = false;

            var explostion = PoolManager.Instance.GetObjectFromPool(explostionEffect);
            explostion.transform.position = transform.position;
            explostion.SetActive(true);

            OnDeath?.Invoke();

            gameObject.SetActive(false);

            eventService.Publish(new ShakeCameraEvent());
        }
    }

    public virtual float GetHealthPresentage()
    {
        return (CurrentHealth / maxHealth);
    }

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
