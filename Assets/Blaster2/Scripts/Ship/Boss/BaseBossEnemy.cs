using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseBossEnemy : BaseEnemy
{
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

        currentWeaponActive = 0;

        DisableAllWeapons();
    }

    public override void TakeDamage(float damage)
    {
        if (GuiManager.Instance.IsTrasnmiting() || delayAttak > 0)
        {
            return;
        }

        hitIndex++;
        OnBossHit?.Invoke(hitIndex, numberOfHits);

        base.TakeDamage(damage);

        if (CurrentHealth < 0)
        {
            EnableColliders(false);
            Instantiate(ExplosionsDeathEffect, transform.position, Quaternion.identity);
        }
    }

    public override void Hit()
    {
        PlayerData playerData = PersistantData.GetPlayerData();

        if (playerData != null)
        {
            playerData.IncreasePowerUp(.05f);
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
        Events.EnemyDied?.Invoke(gameObject.name, this);
    }

    IEnumerator DeathSequence()
    {
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
