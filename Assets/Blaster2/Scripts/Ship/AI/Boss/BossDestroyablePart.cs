using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

public class BossDestroyablePart : Ship, IDamagable,ITargetable
{
    protected Action<bool> Attacked;
    public Ship shipOwner;
    public Enemy_SO EnemyData;
    public GameObject target=>gameObject;
    public bool Targetable=>IsAlive;

    protected EnemyHealthWidget healthBar;
    [SerializeField] protected BossEnemy baseBossEnemy;
    [SerializeField] protected GameObject fireEffect;
    [SerializeField] protected GameObject prepareToAttack;
    [SerializeField] protected BoxCollider boxCollider;

    [Range(.1f, 1)]
    protected float takeDamageDelay;
    public override void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        animator = GetComponentInChildren<Animator>();
    }

    public override void Start()
    {
        IsAlive = true;

        SetStats(shipOwner.Level/4);
    
        fireEffect.SetActive(false);

        if (healthBar != null)healthBar.GetComponent<BaseHealthWidget>();

        if (EnemyData.HealthBarSettings != null)
        {
            GameObject initializedHealthWidget = Instantiate(EnemyData.HealthBarSettings.HealthBarPrefab, transform, false);
            healthBar = (EnemyHealthWidget)initializedHealthWidget.GetComponent<BaseHealthWidget>();
            healthBar.Setup(this, true);
            healthBar.Show();
            initializedHealthWidget.SetActive(true);
        }
    }

    public override void Update()
     {
        if (takeDamageDelay > 0)
        {
            takeDamageDelay -= Time.deltaTime;
        }
    }

    public override void Death()
    {
        IsAlive = false;
        GameObject explostion = PoolManager.Instance.GetObjectFromPool(PoolGameObjectType.ShipExplosion);
        explostion.transform.position = transform.position;
        explostion.SetActive(true);
        fireEffect.SetActive(true);
    }
    public void Attack(Action<bool> callback)
    {
        Attacked = callback;
        StartCoroutine(AttackCoroutine());
    }

    public override void Heal(float ammount)
    {
        CurrentHealth += ammount;
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }

    public override void TakeDamage(float dmg)
    {
        if (IsAlive == false)return;

        AudioManager.PlaySound(EnemyData.hitSFX);
        
        if (takeDamageDelay <= 0)
        {
            takeDamageDelay = .1f;

            CurrentHealth -= dmg;

            OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

            if (CurrentHealth < 1)
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
            destroyable.TakeDamage(shipOwner.GetComponent<BossEnemy>().Damage);
        }
    }

    public override void SetHealth(float health)
    {
        CurrentHealth = health;
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }

    public override void SwitchWeapon(int id, bool solo = false)
    {
      
    }

    public override void EnterLevel()
    {
       
    }

    public override void ExitLevel()
    {
    
    }    
    public override void SetStats(int level)
    {
        Level = level;

        MaxHealth = Level * EnemyData.baseHealth;

        CurrentHealth = MaxHealth;

        Speed = EnemyData.baseSpeed;

        Damage = Level * EnemyData.baseDamage;

        FireRate = EnemyData.baseFireRate;

        HasShield = false;
    }
}
