using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDestroyablePart : MonoBehaviour, IDamagable
{
    public event Action<float, float> OnHealthChanged;
    protected Action<bool> Attacked;

    public bool IsAlive;

    public Enemy_SO enemyData;

    [Range(1, 20)]
    public int Level;
    [Min(12.5f)]
    public float Damage;
    [Min(.3f)]
    public float FireRate;
    [Min(25)]
    public float Speed;
    [Min(0)]
    public float currentHealth;
    [Min(0)]
    public float maxHealth;
    public float CurrentHealth { get { return currentHealth; } }
    public float MaxHealth { get { return maxHealth; } }
    public Animator animator;

    protected EnemyHealthWidget healthBar;


    [SerializeField] protected BaseBossEnemy baseBossEnemy;
    [SerializeField] protected GameObject fireEffect;
    [SerializeField] protected GameObject prepareToAttack;
    [SerializeField] protected BoxCollider boxCollider;

    [Range(.1f, 1)]
    protected float takeDamageDelay;
    public void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        IsAlive = true;

        maxHealth = baseBossEnemy.MaxHealth;
        currentHealth = MaxHealth;

        baseBossEnemy.AddDamagablePart(this);
        fireEffect.SetActive(false);

        animator = GetComponent<Animator>();
        Damage = baseBossEnemy.Damage;

        if (healthBar != null)
        {
            healthBar.GetComponent<BaseHealthWidget>();
        }

        if (enemyData.HealthBarSettings != null)
        {
            GameObject initializedHealthWidget = Instantiate(enemyData.HealthBarSettings.HealthBarPrefab, transform, false);
            healthBar = (EnemyHealthWidget)initializedHealthWidget.GetComponent<BaseHealthWidget>();
            healthBar.Setup(this, true);
            healthBar.Show();
            initializedHealthWidget.SetActive(true);
        }

        SetStats(1);
    }
    private void Update()
    {
        if (takeDamageDelay > 0)
        {
            takeDamageDelay -= Time.deltaTime;
        }
    }

    public void Death()
    {
        GameObject explostion = PoolManager.Instance.GetObjectFromPool(PoolGameObjectType.ShipExplosion);
        explostion.transform.position = transform.position;
        fireEffect.SetActive(true);
    }
    public void Attack(Action<bool> callback)
    {
        Attacked = callback;
        StartCoroutine(AttackCoroutine());
    }

    public void Heal(float ammount)
    {
        currentHealth += ammount;
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }

    public void TakeDamage(float dmg)
    {
        if (IsAlive == false)
        {
            return;
        }

        AudioManager.PlaySound(enemyData.hitSFX);

        if (takeDamageDelay <= 0)
        {
            takeDamageDelay = .1f;

            currentHealth -= dmg;

            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

            if (currentHealth < 1)
            {
                Death();
            }
        }
    }

    IEnumerator AttackCoroutine()
    {
        prepareToAttack.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        animator.SetTrigger("Attack");
        Attacked?.Invoke(true);
        yield return new WaitForSeconds(2.0f);
        Attacked?.Invoke(false);
        prepareToAttack.SetActive(false);
    }

    public void SetStats(int level)
    {
        Level = level;

        maxHealth = Level * enemyData.baseHealth;

        currentHealth = MaxHealth;

        Speed = enemyData.baseSpeed;

        Damage = Level * enemyData.baseDamage;

        FireRate = enemyData.baseFireRate;
    }

    public void EnableColliders(bool enabled)
    {
        if (boxCollider == null)
            boxCollider = GetComponent<BoxCollider>();
        boxCollider.enabled = enabled;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(Constants.PLAYTERTAG))
        {
            var destroyable = other.GetComponent<IDamagable>();
            destroyable.TakeDamage(destroyable.MaxHealth / 2);
            TakeDamage(destroyable.CurrentHealth);
        }
    }

    public float GetHealthPresentage()
    {
        return (CurrentHealth / MaxHealth) * 100;
    }

    public void SetHealth(float health)
    {
        currentHealth = health;
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }
}
