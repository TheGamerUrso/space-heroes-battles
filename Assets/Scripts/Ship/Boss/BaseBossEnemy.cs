using System;
using TheGamerUrso;
using UnityEngine;
using System.Linq;

using Random = UnityEngine.Random;
using System.Collections;

public class BaseBossEnemy : BaseEnemy
{
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

    protected int phase;


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
        for (int weaponIndex = 0; weaponIndex < Weapons.Length; weaponIndex++)
        {
            Weapons[weaponIndex].GetComponent<WeaponScript>().SetShipStatsSystem(shipStatsSystem);
        }

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

        for (int i = 0; i < Weapons.Length; i++)
        {
            Weapons[i].GetComponent<WeaponScript>().SetShipStatsSystem(shipStatsSystem);
            Weapons[i].SetActive(false);
        }
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

        if (shipStatsSystem.CurrentHealth < 0)
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



        if (Weapons[0].activeSelf == false)
        {
            Weapons[0].SetActive(true);
        }
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
            for (int i = 0; i < Weapons.Length; i++)
            {
                Weapons[i].SetActive(false);
            }
        }

        if (shipStatsSystem.CurrentHealth > 0)
        {
            Attack();
        }


    }

    public virtual void Phases()
    {
        if (phase == 0 && shipStatsSystem.GetHealthPressentage() <= 70f)
        {
            phase = 1;
            for (int i = 0; i < Weapons.Length; i++)
            {
                float newFireRate = Weapons[i].GetComponent<WeaponScript>().FireRate - .2f;

                Weapons[i].GetComponent<WeaponScript>().FireRate = newFireRate;
            }
        }
        else if (phase == 1 && shipStatsSystem.GetHealthPressentage() <= 30f)
        {
            phase = 2;
            for (int i = 0; i < Weapons.Length; i++)
            {
                float newFireRate = Weapons[i].GetComponent<WeaponScript>().FireRate - .2f;

                Weapons[i].GetComponent<WeaponScript>().FireRate = newFireRate;
            }

        }
        else if (phase == 2 && shipStatsSystem.GetHealthPressentage() <= 10f)
        {
            phase = 3;
            for (int i = 0; i < Weapons.Length; i++)
            {
                float newFireRate = Weapons[i].GetComponent<WeaponScript>().FireRate - .2f;

                Weapons[i].GetComponent<WeaponScript>().FireRate = newFireRate;
            }
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

            for (int i = 0; i < Weapons.Length; i++)
            {
                if (!Weapons[i].activeSelf)
                {
                    Weapons[i].SetActive(true);

                }
            }

            Phases();

        }
    }

    public override void Death()
    {
        var info = animator.GetCurrentAnimatorStateInfo(0);
        animator.SetBool("Death", true);

        StartCoroutine(DeathSequence());

        EnableColliders(false);

        for (int i = 0; i < currentWeaponActive; i++)
        {
            Weapons[i].SetActive(false);
        }

        EnemyProjectile[] enemyProjectiles = GameObject.FindObjectsOfType<EnemyProjectile>();
        if (enemyProjectiles.Length > 0)
        {
            foreach (EnemyProjectile item in enemyProjectiles)
            {
                item.gameObject.SetActive(false);
            }
        }

        GuiManager.Instance.ToggleSlowMo(false); 
    }

    IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(4.0f);
        base.Death();
    }

}
