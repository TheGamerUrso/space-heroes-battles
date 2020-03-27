using System;
using System.Collections;
using TheGamerUrso.PoolSystem;
using UnityEngine;
using DG.Tweening;
public class Punch : MonoBehaviour, IDestroyable
{
    public Action<float, float> HealthChanged;
    public Action<bool> Attacked;

    public BaseBossEnemy baseBossEnemy;
    public bool isAlive;
    public GameObject fireEffect;

    public Animator animator;
    public float damage;

    public GameObject prepareToAttack;

    public bool IsDestroyed
    {
        get
        {
            return !isAlive;
        }
        set
        {
            isAlive = value;
        }
    }

    public float maxHealth = 100;
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

    public float currentHealth;
    public float CurrentHealth
    {
        get
        {
            return currentHealth;
        }
        set
        {
            currentHealth = value;
        }
    }

    public float HealthPresentage
    {
        get
        {
            return currentHealth / maxHealth;
        }
    }

    [SerializeField] private HealthBarSettings HealthBarSettings;
    private EnemyHealthWidget healthBar;
    public EnemyHealthWidget HealthBar
    {
        get
        {
            return healthBar;
        }
    }

    public Action<float, float> OnHealthChange
    {
        get
        {
            return HealthChanged;
        }
        set
        {
            HealthChanged = value;
        }
    }

    private void Start()
    {
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
            healthBar.Setup(this, false);
            healthBar.Show();
            initializedHealthWidget.SetActive(true);
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
            currentHealth -= dmg;
            if (currentHealth <= 0)
            {
                isAlive = false;
                currentHealth = 0;
                GameObject explostion = PoolManager.Instance.GetObjectFromPool(PoolGameObjectType.ShipExplosion);
                explostion.transform.position = transform.position;
                fireEffect.SetActive(true);
            }
            OnHealthChange?.Invoke(currentHealth, maxHealth);
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


}