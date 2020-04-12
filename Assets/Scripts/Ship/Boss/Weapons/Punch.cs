using System;
using System.Collections;
using TheGamerUrso.PoolSystem;
using UnityEngine;
using DG.Tweening;
public class Punch : MonoBehaviour, IDestroyable
{
    protected bool Alive;
    public bool IsDestroyed
    {
        get
        {
            return Alive;
        }
        set
        {
            Alive = value;
        }
    }

    /**
   * Attributes
   */
    #region Attributes
    public delegate void HealthChanged(float currentHealth, float maxHealth);
    public event HealthChanged OnHealthChanged;


    [Min(25)]
    public float maxHealth;

    [Min(0)]
    public float currentHealth;

    [Min(12.5f)]
    public float Damage;

    public float CurrentHealth
    {
        get
        {
            return currentHealth;
        }
        set
        {
            currentHealth = value;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }
    }

    public float MaxHealth
    {
        get
        {
            return maxHealth;
        }
        set
        {
            maxHealth = value;
        }
    }

    public float HealthPresentage
    {
        get
        {
            return (CurrentHealth / MaxHealth) * 100;
        }
    }
    #endregion Attributes

    public Action<bool> Attacked;

    public BaseBossEnemy baseBossEnemy;
    public bool isAlive;
    public GameObject fireEffect;

    public float damage;

    public GameObject prepareToAttack;

    protected Animator animator;

    [Header("Effects")]
    [SerializeField] protected PoolGameObjectType ExplostionEffect;

    [SerializeField] private HealthBarSettings HealthBarSettings;
    private EnemyHealthWidget healthBar;
    public EnemyHealthWidget HealthBar
    {
        get
        {
            return healthBar;
        }
    }

    public void Attack(Action<bool> callback)
    {
        Attacked = callback;
        StartCoroutine(AttackCoroutine());
    }

    IEnumerator AttackCoroutine()
    {
        prepareToAttack.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        animator.SetTrigger("Attack");      
        Attacked?.Invoke(true);
        yield return new WaitForSeconds(2.0f);
        Attacked?.Invoke(false);
        prepareToAttack.SetActive(false);

    }

    public void TakeDamage(float dmg)
    {
        if (isAlive)
        {
            CurrentHealth -= dmg;
            if (currentHealth <= 0)
            {
                isAlive = false;
                currentHealth = 0;
                GameObject explostion = PoolManager.Instance.GetObjectFromPool(PoolGameObjectType.ShipExplosion);
                explostion.transform.position = transform.position;
                fireEffect.SetActive(true);
            }
   
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        string gameobjectTag = other.gameObject.tag;

        if (gameobjectTag.Equals(Constants.PLAYTERTAG))
        {
            IDestroyable destroyable = other.GetComponent<IDestroyable>();
            if (destroyable != null)
            {
                destroyable.TakeDamage(damage);
            }
        }
    }

    private void Awake()
    {
        OnAwake();
    }

    private void Start()
    {
        ShipSetup();
    }

    public void ShipSetup()
    {
        maxHealth = baseBossEnemy.MaxHealth;
        currentHealth = maxHealth;
        baseBossEnemy.AddDamagablePart(this);
        fireEffect.SetActive(false);
        isAlive = true;
        animator = GetComponent<Animator>();

        if (healthBar != null)
        {
            healthBar.GetComponent<BaseHealthWidget>();
        }

        if (HealthBarSettings != null)
        {
            GameObject initializedHealthWidget = Instantiate(HealthBarSettings.HealthBarPrefab, transform, false);
            healthBar = (EnemyHealthWidget)initializedHealthWidget.GetComponent<BaseHealthWidget>();
            healthBar.Setup(this, true);
            healthBar.Show();
            initializedHealthWidget.SetActive(true);
        }
    }

    public void OnAwake()
    {
      
    }

    public void Death()
    {

    }
}