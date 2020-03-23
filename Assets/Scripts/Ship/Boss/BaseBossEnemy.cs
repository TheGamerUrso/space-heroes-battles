using System;
using TheGamerUrso;
using UnityEngine;
using System.Linq;

using Random = UnityEngine.Random;
using System.Collections;
using TheGamerUrso.Utils;

public class BaseBossEnemy : BaseEnemy
{
    public Action OnBossAttack;

    #region Animation Config
    [Header("Animation Config")]
    int enterNameHash = Animator.StringToHash("Enter");
    int flyingNameHash = Animator.StringToHash("Flying");
    int deathNameHash = Animator.StringToHash("Death");
    #endregion

    #region Boss Config
    [Header("Boss Config")]
    public BaseBossEnemyAI BossAI;
    protected GameObject bossWidget;
    protected int hitIndex;
    protected int numberOfHits;
    public GameObject ExplosionsDeathEffect;

    #endregion

    #region Destroyable Parts Cofig
    [Header("Destroyable Parts Cofig")]
    [SerializeField] protected IDestroyable[] DestroyableParts;
    #endregion

    public override void Enter()
    {
        EnableColliders(false);
        currentWeaponActive = 1;
    }

    public override void InitReferences()
    {
        base.InitReferences();
        BossAI = GetComponent<BaseBossEnemyAI>();
        DeathDelay = AnimUtil.GetSpecificAnimatorClipLength(animator, "Death");
        DestroyableParts = transform.GetComponentsInChildren<IDestroyable>().Where((item) => !item.Equals(this)).ToArray();

        for (int i = 0; i < DestroyableParts.Length; i++)
        {
            MonoBehaviour go = DestroyableParts[i] as MonoBehaviour;
            if (go != this)
            {
                DestroyableParts[i].MaxHealth = MaxHealth / 2;
                DestroyableParts[i].CurrentHealth = DestroyableParts[i].MaxHealth;
            }
        }

        currentWeaponActive = 0;

        DisableWeapons();
    }

    public override void TakeDamage(float damage)
    {
        if (GuiManager.IsTrasnmiting() || delayAttak > 0)
        {
            return;
        }

        foreach (IDestroyable item in DestroyableParts)
        {
            if (item.IsDestroyed == false)
            {
                return;
            }
        }

        BossHit();

        base.TakeDamage(damage);

        if (shipStatsSystem.currentHealth < 0)
        {
            EnableColliders(false);
            Instantiate(ExplosionsDeathEffect, transform.position, Quaternion.identity);
        }
    }

    public virtual void BossHit()
    {
        PlayerWeaponSystem playerWeaponSystem = GameObject.FindObjectOfType<PlayerWeaponSystem>();
        playerWeaponSystem.IncreasePowerUp(.05f);


        hitIndex++;

        if (hitIndex > numberOfHits)
        {
            hitIndex = 0;
            BossAI.ChangeWaypoint(hitIndex);
        }



        EnableWeapon();
    }

    public override void Tick()
    {
        var info = animator.GetCurrentAnimatorStateInfo(0);

        if (info.shortNameHash == enterNameHash || GuiManager.IsTrasnmiting())
        {
            return;
        }

        if (info.shortNameHash == deathNameHash)
        {
            DisableWeapons();
        }

        if (CurrentHealth > 0)
        {
            Attack();
        }


    }

    public override void Attack()
    {
        if (delayAttak > 0)
        {
            delayAttak -= Time.deltaTime;
        }
        else
        {

            EnableWeapon();

            OnBossAttack?.Invoke();

        }
    }

    public override void Death()
    {
        var info = animator.GetCurrentAnimatorStateInfo(0);
        animator.SetBool("Death", true);

        StartCoroutine(DeathSequence());

        EnableColliders(false);

        DisableWeapons();

        EnemyProjectile[] enemyProjectiles = GameObject.FindObjectsOfType<EnemyProjectile>();
        if (enemyProjectiles.Length > 0)
        {
            foreach (EnemyProjectile item in enemyProjectiles)
            {
                item.gameObject.SetActive(false);
            }
        }

        GameController.useSloMo = false;
    }

    IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(4.0f);
        base.Death();
    }

    public float GetHealtHPresentage()
    {
        return shipStatsSystem.GetHealthPressentage();
    }

    public void SetFireRate(int weaponIndex = 0,bool all = true)
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

}
