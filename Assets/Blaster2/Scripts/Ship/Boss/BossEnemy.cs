using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.MPE;
using UnityEngine;

public class BossEnemy : Enemy
{
    public Action<int> OnBossPhaseChanged;
    public event Action<BossEnemy,int> OnBossAttacked;
    #region Components
    [SerializeField] protected List<BossDestroyablePart> DestroyableParts = new List<BossDestroyablePart>();
    #endregion

    public bool StartBattle { get; protected set; }
    [SerializeField] private int Phase;



    public override void OnEnable()
    {
        base.OnEnable();
        EnableColliders(false);
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        OnBossPhaseChanged -= OnBossPhaseChangedHandled;
    }

    public override void Awake()
    {
        base.Awake();
        OnBossPhaseChanged -= OnBossPhaseChangedHandled;
    }

    public override void Start()
    {
        base.Start();
        baseEnemyMovement.Speed = Speed;
        Phase = 1;
    }

    public override void SetEnemyHealthUI()
    {
        if (HealthBar != null)
        {
            HealthBar.GetComponent<BaseHealthWidget>();
        }
        if (EnemyData.HealthBarSettings != null)
        {
            GameObject initializedHealthWidget = Instantiate(EnemyData.HealthBarSettings.HealthBarPrefab, transform, false);
            HealthBar = initializedHealthWidget.GetComponent<BaseHealthWidget>();
            HealthBar.GetComponent<EnemyHealthWidget>().Setup(this, false);
        }
    }

    public override void Update()
    {
        base.Update();
        if (CurrentHealth > 0)
        {
            Attack();
        }
    }
    public override void ExitLevel()
    {

    }

    public override void TakeDamage(float dmg)
    {
        if (GameController.Instance.CurrentGameState == GameState.TRANSMISSION || delayAttak > 0) return;
        if (IsAlive == false) return;

        ShieldEffect.SetActive(IsProtected());

        audioService.PlaySound(EnemyData.hitSFX);

        if (takeDamageDelay <= 0)
        {
            takeDamageDelay = .1f;

            foreach (var part in DestroyableParts)
            {
                if (part.IsAlive)
                {
                    return;
                }
            }
            if (HasShield)
            {
                HasShield = false;
            }
            else if (HasShield == false)
            {
                CurrentHealth -= dmg;

                OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

                if (CurrentHealth < 1)
                {
                    Death();
                }
                IncreasePhase();
            }
            ShieldEffect.SetActive(HasShield);
        }
        Hit();
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

    public void Attack()
    {
        if (delayAttak > 0)
        {
            delayAttak -= Time.deltaTime;
        }
        else
        {
            OnBossAttacked?.Invoke(this,hitIndex);
        }
    }

    IEnumerator DeathSequence()
    {
        DeathExplosions();

        EnableColliders(false);

        DisableAllWeapons();

        DestroyOwnedProjectiles();

        if (HealthBar != null)
        {
            Destroy(HealthBar);
        }

        yield return new WaitForSeconds(4.0f);

        for (int i = 0; i < 4; i++)
        {
            eventService?.Publish(new DropRandomItemEvent() { SpawnPosition = transform });
        }

        if (IsAlive)
        {
            IsAlive = false;
            var explostion = PoolManager.Instance.GetObjectFromPool(EnemyData.ExplostionEffect);
            explostion.transform.position = transform.position;
            explostion.SetActive(true);
            HealthBar.Hide();
            eventService?.Publish(new ShakeCameraEvent() { duration = .5f});
            eventService?.Publish(new DropRandomItemEvent() { SpawnPosition = transform });
            Destroy(transform.parent.gameObject);
        }
    }

    protected override void DestroyOwnedProjectiles()
    {
        EnemyProjectile[] enemyProjectiles = GameObject.FindObjectsOfType<EnemyProjectile>();
        if (enemyProjectiles.Length > 0)
        {
            foreach (EnemyProjectile item in enemyProjectiles)
            {
                item.gameObject.SetActive(false);
            }
        }
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
            var explostion = PoolManager.Instance.GetObjectFromPool(EnemyData.ExplostionEffect);
            explostion.transform.position = positions[i];
            explostion.SetActive(true);
        }
    }

    private bool IsProtected()
    {
        int destroyed = 0;
        if (DestroyableParts.Count > 0)
        {
            if (destroyed < DestroyableParts.Count)
            {
                return true;
            }
            else
            {
                return true;
            }

        }
        else
        {
            return false;
        }
    }

    public override IEnumerator DelayStart()
    {
        HealthBar.Show();
        yield return new WaitForSeconds(4);
        EnableAllWeapon();
        EnableColliders(true);
        StartBattle = true;
        baseEnemyMovement.EnableMovement();
    }

    public void IncreasePhase()
    {
        if (GetHealthPresentage() < 50f && Phase != 2)
        {
            Phase = 2;
            OnBossPhaseChanged?.Invoke(Phase);
        }
        else if (GetHealthPresentage() < 25f && Phase != 3)
        {
            Phase = 3;
            OnBossPhaseChanged?.Invoke(Phase);
        }

    }

    public virtual void OnBossPhaseChangedHandled(int Phase)
    {
        SetFireRate(0.2f);
    }
}
