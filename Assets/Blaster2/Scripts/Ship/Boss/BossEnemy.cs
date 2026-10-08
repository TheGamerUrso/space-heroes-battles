using System;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : Enemy
{
    [SerializeField] protected Animator animator;
    [SerializeField] protected List<BossDestroyablePart> DestroyableParts = new List<BossDestroyablePart>();
    public Action<int> OnBossPhaseChanged;

    public bool StartBattle { get; protected set; }
    public int Phase { get; set; } = 1;

    public override void Enter()
    {
       healthComponent.Isinvulnerable = true;
        if (animator == null) return;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // Check if we are currently playing the Enter animation and it has reached or passed 100% completion
        if (stateInfo.IsName("Flying") && stateInfo.normalizedTime >= 1.0f)
        {
            healthComponent.SetDamagable(true);
            weaponController.SetWeapon(0);
            EnemyState = EnemyState.Combat;
            healthComponent.Isinvulnerable = false;
        }
    }

    public override void Idle()
    {
        base.Idle();
        animator.SetBool("Death", false);
    }

    public override void Combat()
    {
        base.Combat();
    }


    protected override void OnHealthValueChanged(float currentHealth, float MaxHealth)
    {
        base.OnHealthValueChanged(currentHealth, MaxHealth);
        var healthPercentage = currentHealth / MaxHealth;
        int newPhase = Phase;

        if (healthPercentage <= 0.20f) newPhase = 4;
        else if (healthPercentage <= 0.50f) newPhase = 3;
        else if (healthPercentage <= 0.75f) newPhase = 2;
        else newPhase = 1;

        // Only update and invoke if the phase actually changes
        if (newPhase != Phase)
        {
            Phase = newPhase;
            OnBossPhaseChanged?.Invoke(Phase);

            // Lower delay = faster fire rate
            float newFireRate = Phase switch
            {
                1 => 1.5f,  // Phase 1: 1.5s delay (slower start)
                2 => 1.2f,  // Phase 2: 1.2s delay
                3 => 0.9f,  // Phase 3: 0.9s delay
                4 => 0.6f,  // Phase 4: 0.6s delay (rapid fire)
                _ => 1.5f
            };

            weaponController.SetFireRate(newFireRate);
        }
    }

    public override void Death()
    {
        eventService.Publish(new EnemyEvent()
        {
            Type = EnemyEvent.EnemyEventType.DEATH
        ,
            Enemy = this,
            Value = ship_SO.EnemyValue
        });

        SetState(EnemyState.Idle);

        var explostion = PoolManager.Instance.GetObjectFromPool(ship_SO.ExplostionEffect);
        explostion.transform.position = transform.position;
        explostion.SetActive(true);

        eventService.Publish(new ShakeCameraEvent());
        gameObject.SetActive(false);
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
