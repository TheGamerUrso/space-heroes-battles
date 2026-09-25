using System;
using UnityEngine;

public class BossEnemy : Enemy
{
    public Action<int> OnBossPhaseChanged;

    public bool StartBattle { get; protected set; }
    [SerializeField] private int Phase;
    private float enterStartDelay = 4;


    public override void Start()
    {
        base.Start();
        Phase = 1;

        healthComponent.OnHealthChanged += OnHealthValueChanged;
    }
    private void OnDestroy()
    {
        healthComponent.OnHealthChanged -= OnHealthValueChanged;
    }

    public override void Update()
    {
        switch (enemyState)
        {
            case EnemyState.None:
                break;
            case EnemyState.Idle:
                break;
            case EnemyState.Enter:
                enterStartDelay -= Time.deltaTime;
                if (enterStartDelay <= 0) 
                {
                    weaponController.EnableAllWeapon();
                    StartBattle = true;
                }
                break;
            case EnemyState.Combat:
                break;
            case EnemyState.Hit:
                break;
            case EnemyState.Escape:
                OnEnemyEscaped?.Invoke(this);
                gameObject.SetActive(false);
                break;
            case EnemyState.Death:
                gameObject.SetActive(false);
                break;
        }
    }

    private bool IsProtected()
    {
        //int destroyed = 0;
        //if (DestroyableParts.Count > 0)
        // {
        //    if (destroyed < DestroyableParts.Count)
        //    {
        //         return true;
        //     }
        //     else
        //     {
        //          return true;
        //      }
        ///
        //   }
        //  else
        //   {
        //       return false;
        //  }
        return false;
    }

    public void OnHealthValueChanged(float currentHealth,float MaxHealth)
    {
        if (healthComponent.GetHealthPresentage() < 50f && Phase != 2)
        {
            Phase = 2;
            OnBossPhaseChanged?.Invoke(Phase);
            weaponController.SetFireRate(0.2f);
        }
        else if (healthComponent.GetHealthPresentage() < 25f && Phase != 3)
        {
            Phase = 3;
            OnBossPhaseChanged?.Invoke(Phase);
            weaponController.SetFireRate(0.2f);
        }
    }
}
