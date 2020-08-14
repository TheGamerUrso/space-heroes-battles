using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseBossEnemy : BaseEnemy
{
    private PlayerData playerData;
    public Action OnBossAttack;
    public Action<int, int> OnBossHit;

    protected int hitIndex;
    protected int numberOfHits;
    [SerializeField] protected GameObject ExplosionsDeathEffect;
    [SerializeField] protected List<IDamagable> DestroyableParts = new List<IDamagable>();


    public override void OnEnable()
    {
        base.OnEnable();
        EnableColliders(false);
        currentWeaponActive = 1;
    }

    public override void Start()
    {
        base.Start();
        playerData = PersistantData.GetPlayerData();

        currentWeaponActive = 0;
        DisableAllWeapons();
    }

    public override void TakeDamage(float damage)
    {
        if (GuiManager.Instance.IsTrasnmiting() || delayAttak > 0)
        {
            return;
        }

        base.TakeDamage(damage);

    }

    public override void Hit()
    {
        hitIndex++;
        OnBossHit?.Invoke(hitIndex, numberOfHits);
        playerData.SetSuperMeter(playerData.PowerUpLevel + 0.5f);
    }

    public override void Update()
    {
        base.Update();
        if (CurrentHealth > 0)
        {
            Attack();
        }
    }

    public void Attack()
    {
        if (delayAttak > 0)
        {
            delayAttak -= Time.deltaTime;
        }
        else
        {
            OnBossAttack?.Invoke();
        }
    }

    public override void Death()
    {
        var info = animator.GetCurrentAnimatorStateInfo(0);
        animator.SetBool("Death", true);

        StartCoroutine(DeathSequence());

        EnableColliders(false);

        DisableAllWeapons();

        EnemyProjectile[] enemyProjectiles = GameObject.FindObjectsOfType<EnemyProjectile>();
        if (enemyProjectiles.Length > 0)
        {
            foreach (EnemyProjectile item in enemyProjectiles)
            {
                item.gameObject.SetActive(false);
            }
        }

        if (HealthBar != null)
        {
            Destroy(HealthBar);
        }

        Events.BossDied?.Invoke(Id, this);
    }

    IEnumerator DeathSequence()
    {
        if (CurrentHealth < 0)
        {
            EnableColliders(false);

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

        yield return new WaitForSeconds(4.0f);
        base.Death();
    }

    public void SetFireRate(int weaponIndex = 0, bool all = true)
    {
        if (all)
        {
            for (int i = 0; i < Weapons.Length; i++)
            {
                float newFireRate = Weapons[i].FireRate - .2f;

                Weapons[i].FireRate = newFireRate;
            }
        }
        else
        {
            float newFireRate = Weapons[weaponIndex].FireRate - .2f;

            Weapons[weaponIndex].FireRate = newFireRate;
        }
    }

    public void AddDamagablePart(IDamagable part)
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

}
