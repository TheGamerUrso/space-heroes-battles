using System;
using System.Collections;
using TheGamerUrso.PoolSystem;
using UnityEngine;
using DG.Tweening;
public class Punch : MonoBehaviour, IDestroyable
{
    public Action<bool> Attacked;

    public BaseBossEnemy baseBossEnemy;
    public bool isAlive;
    public GameObject fireEffect;

    public Animator animator;
    public float damage;
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


    private void Start()
    {
        currentHealth = maxHealth;
        baseBossEnemy.AddDamagablePart(this);
        fireEffect.SetActive(false);
        isAlive = true;
        animator = GetComponent<Animator>();
 

    }

    public void Attack(Action<bool> callback)
    {
        Attacked = callback;
        StartCoroutine(AttackCoroutine());
    }

    IEnumerator AttackCoroutine()
    {
        animator.SetTrigger("Attack");
        Attacked?.Invoke(true);
        yield return new WaitForSeconds(2.0f);
        Attacked?.Invoke(false);

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


}