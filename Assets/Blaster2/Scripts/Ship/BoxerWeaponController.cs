using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class BoxerPunches 
{
    public HealthComponent healthComponent;
    public GameObject fireEffect;
    public GameObject prepareToAttack;
}


public class BoxerWeaponController : WeaponController
{
    protected Action<bool> Attacked;
    public Animator animator;
  
    [Range(.1f, 1)]
    protected float takeDamageDelay;
    [Header("Movement")]
    public bool attacking;
    private float cooldown;
    public BoxerPunches[] punches;

    public void Awake()
    {
        foreach (var item in punches)
        {
            item.fireEffect.SetActive(false);
        }
    }

    protected override void Start()
    {
        base.Start();
        foreach (var item in punches)
        {
            item.healthComponent.OnHealthChanged += (x,y) =>{
                if (x <= 0)
                {
                    item.fireEffect.SetActive(true);
                }
            };
        }
    }

    private void OnDestroy()
    {
        foreach (var item in punches)
        {
            item.healthComponent.OnHealthChanged = null;
        }
    }

    protected override void Attack()
    {
        if (!attacking && cooldown > 0)
        {
            cooldown -= Time.deltaTime;
        }

        if (cooldown <= 0)
        {
            if (punches.Length > 0)
            {
                List<BoxerPunches> newList = punches.Where(x => x.healthComponent.CurrentHealth > 0).ToList();
                int rand = UnityEngine.Random.Range(0, newList.Count);
                if (newList.Count > 0)
                {
                    cooldown = UnityEngine.Random.Range(4, 8);
                    Punch(newList[rand]);
                }
            }
        }
    }

    public void Punch(BoxerPunches punche)
    {
        attacking = true;
        StartCoroutine(AttackCoroutine(punche));
    }

    IEnumerator AttackCoroutine(BoxerPunches punche)
    {
        punche.prepareToAttack.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        animator.SetTrigger("Attack");
        Attacked?.Invoke(true);
        yield return new WaitForSeconds(2.0f);
        Attacked?.Invoke(false);
        punche.prepareToAttack.SetActive(false);
        attacking = false;
    }
}
