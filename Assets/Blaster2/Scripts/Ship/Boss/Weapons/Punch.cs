using System;
using System.Collections;
using UnityEngine;
public class Punch : BaseEnemy, IDamagable
{
    public Action<bool> Attacked;
    public BaseBossEnemy baseBossEnemy;
    public GameObject fireEffect;
    public GameObject prepareToAttack;

    public override void Start()
    {
        base.Start();

        IsAlive = true;

        MaxHealth = baseBossEnemy.MaxHealth;
        CurrentHealth = MaxHealth;

        baseBossEnemy.AddDamagablePart(this);
        fireEffect.SetActive(false);
   
        animator = GetComponent<Animator>();
        Damage = baseBossEnemy.Damage;

        if (HealthBar != null)
        {
            HealthBar.GetComponent<BaseHealthWidget>();
        }

        if (EnemyData.HealthBarSettings != null)
        {
            GameObject initializedHealthWidget = Instantiate(EnemyData.HealthBarSettings.HealthBarPrefab, transform, false);
            HealthBar = (EnemyHealthWidget)initializedHealthWidget.GetComponent<BaseHealthWidget>();
            HealthBar.Setup(this, true);
            HealthBar.Show();
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
        yield return new WaitForSeconds(2.0f);
        animator.SetTrigger("Attack");
        Attacked?.Invoke(true);
        yield return new WaitForSeconds(2.0f);
        Attacked?.Invoke(false);
        prepareToAttack.SetActive(false);

    }

    public override void Death()
    {
        GameObject explostion = PoolManager.Instance.GetObjectFromPool(PoolGameObjectType.ShipExplosion);
        explostion.transform.position = transform.position;
        fireEffect.SetActive(true);
    }

    public override void Heal(float ammount)
    {
       
    }
}