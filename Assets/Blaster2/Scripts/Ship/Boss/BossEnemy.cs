using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class BossEnemy : Enemy
{
    [SerializeField] protected List<BossDestroyablePart> DestroyableParts = new List<BossDestroyablePart>();
    public Action<int> OnBossPhaseChanged;

    public bool StartBattle { get; protected set; }
    public int Phase { get; set; } = 1;

    public override void Enter()
    {
        if (animator == null)
        {
            return;
        }

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // Check if we are currently playing the Enter animation and it has reached or passed 100% completion
        if (stateInfo.IsName("Flying") && stateInfo.normalizedTime >= 1.0f)
        {
            healthComponent.SetDamagable(true);
            weaponController.SetWeapon(0);
            EnemyState = EnemyState.Combat;
        }
    }

    public override void Combat()
    {
        weaponController.ShouldAttack = true;
        var currentWeapon = weaponController.GetCurrentWeapon();
        if (currentWeapon != null)
        {
            currentWeapon.Shoot();
        }
    }

    private void OnDestroy()
    {
        healthComponent.OnHealthChanged -= OnHealthValueChanged;
    }

    protected override void OnHealthValueChanged(float currentHealth, float MaxHealth)
    {
        var healthPresentage = currentHealth / MaxHealth;

        if (currentHealth < 1)
            Death();

        if (shipData.HasShield)
            DeactivateShield();

        if (healthPresentage < .5f && Phase != 2)
        {
            Phase = 2;
            OnBossPhaseChanged?.Invoke(Phase);
            weaponController.SetFireRate(0.2f);
        }
        else if (healthPresentage < .25f && Phase != 3)
        {
            Phase = 3;
            OnBossPhaseChanged?.Invoke(Phase);
            weaponController.SetFireRate(0.2f);
        }

        Hit();
    }

    public override void Death()
    {
        if (IsDead) return;
        IsDead = true;

        eventService.Publish(new EnemyEvent()
        {
            Type = EnemyEvent.EnemyEventType.DEATH
        ,
            Enemy = this,
            Value = ship_SO.EnemyValue
        });

        animator.SetBool("Death", true);


        eventService.Publish(new ShakeCameraEvent());
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        DeathExplosions();

        healthComponent.SetDamagable(false);

        yield return new WaitForSeconds(4.0f);

        for (int i = 0; i < 4; i++)
        {
            eventService?.Publish(new DropRandomItemEvent() { SpawnPosition = transform });
        }

        var explostion = PoolManager.Instance.GetObjectFromPool(explostionEffect);
        explostion.transform.position = transform.position;
        explostion.SetActive(true);
        eventService?.Publish(new ShakeCameraEvent() { duration = .5f });
        eventService?.Publish(new DropRandomItemEvent() { SpawnPosition = transform });
        Destroy(transform.parent.gameObject);

        SetState(EnemyState.Death);
    }

    //Boss Owned Methods
    private void DeathExplosions()
    {
        Vector3[] positions ={
                 transform.position,
                transform.position + (transform.right * 50),
                  transform.position - (transform.right * 50),
                    transform.position + (transform.forward * 50),
                      transform.position - (transform.forward * 50)
            };

        for (int i = 0; i < 5; i++)
        {
            var explostion = PoolManager.Instance.GetObjectFromPool(explostionEffect);
            explostion.transform.position = positions[i];
            explostion.SetActive(true);
        }
    }


    public bool IsProtected()
    {
        foreach (var part in DestroyableParts)
        {
            if (part.healthComponent.IsAlive)
            {
                return true;
            }
        }
        return false;
    }
}
