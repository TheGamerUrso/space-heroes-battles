using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class BossEnemy : Enemy
{
    [SerializeField] protected List<HealthComponent> DestroyableParts = new List<HealthComponent>();

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
            case EnemyState.Escape:
                OnEnemyEscaped?.Invoke(this);
                gameObject.SetActive(false);
                break;
            case EnemyState.Death:
                gameObject.SetActive(false);
                break;
        }
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
    public override void Hit()
    {
        base.Hit();
        playerData.SetSuperMeter(playerData.PowerUpLevel + 0.15f);
    }

    public override void Death()
    {
        animator.SetBool("Death", true);
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
            if (part.IsAlive)
            {
                return true;
            }
        }
        return false;
    }
}
