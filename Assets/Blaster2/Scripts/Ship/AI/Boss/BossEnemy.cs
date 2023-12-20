using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossEnemy : Enemy, IDamagable, ITargetable
{
    #region Components
    [SerializeField] protected List<IDamagable> DestroyableParts = new List<IDamagable>();
    #endregion

    [SerializeField] protected bool StartBattle;

    public override void OnEnable()
    {
        base.OnEnable();
        EnableColliders(false);
    }

    public override void Start()
    {
        base.Start();
        baseEnemyMovement.Speed = Speed;
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
            HealthBar.GetComponent<EnemyHealthWidget>().Setup(this, true);
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
        if (GuiManager.Instance.IsTrasnmiting() || delayAttak > 0) return;
        if (IsAlive == false) return;

        ShieldEffect.SetActive(IsProtected());

        AudioManager.PlaySound(EnemyData.hitSFX);

        if (takeDamageDelay <= 0)
        {
            takeDamageDelay = .1f;

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
        Events.BossDied?.Invoke(Id, this);
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
            OnEnemyAttack?.Invoke();
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
            DropItem.Instance.PickRandomDropItem(transform);
        }

        if (IsAlive)
        {
            IsAlive = false;
            var explostion = PoolManager.Instance.GetObjectFromPool(EnemyData.ExplostionEffect);
            explostion.transform.position = transform.position;
            explostion.SetActive(true);
            Events.BossDied?.Invoke(Id, this);
            HealthBar.Hide();

            Events.ShakeCamera?.Invoke(.5f);
            DropItem.Instance.PickRandomDropItem(transform);

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

    public override void AddDamagablePart(IDamagable part)
    {
        DestroyableParts.Add(part);

        MonoBehaviour go = part as MonoBehaviour;
        if (go != this)
        {
            BossDestroyablePart partGO = go.GetComponent<BossDestroyablePart>();
            partGO.maxHealth = part.MaxHealth / 2;
            float health = part.MaxHealth;
            partGO.SetHealth(health);
        }
    }

    public override void OnEnemyHitHandled(int hitIndex, int numberOfHits)
    {
        if (GuiManager.Instance.IsTrasnmiting())
        {
            return;
        }

        PlayerData playerData = PersistantData.GetPlayerData();

        if (playerData != null)
        {
            playerData.SetSuperMeter(playerData.PowerUpLevel + 0.05f);
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
        yield return new WaitForSeconds(2);
        EnableAllWeapon();
        EnableColliders(true);
        StartBattle = true;
        baseEnemyMovement.EnableMovement();
    }
}
