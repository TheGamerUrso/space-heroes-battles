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
    public Action<int, int> OnBossHit;

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

    public override void OnAwake()
    {
        base.OnAwake();
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

        DisableAllWeapons();
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

        hitIndex++;
        OnBossHit?.Invoke(hitIndex, numberOfHits);

        base.TakeDamage(damage);

        if (shipStatsSystem.currentHealth < 0)
        {
            EnableColliders(false);
            Instantiate(ExplosionsDeathEffect, transform.position, Quaternion.identity);
        }
    }

    public virtual void BossHit()
    {
        PlayerShip playerShip = PlayerManager.GetPlayer();
        playerShip.IncreasePowerUp(.05f);
       
    }

    public override void OnUpdate()
    {
        var info = animator.GetCurrentAnimatorStateInfo(0);

        if (info.shortNameHash == enterNameHash || GuiManager.IsTrasnmiting())
        {
            return;
        }

        if (info.shortNameHash == deathNameHash)
        {
            DisableAllWeapons();
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

        EnemyDied?.Invoke(gameObject.name, this);

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
