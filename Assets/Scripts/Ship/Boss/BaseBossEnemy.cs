using System;
using TheGamerUrso;
using UnityEngine;
using System.Linq;

public class BaseBossEnemy : BaseEnemy
{
    [Header("Boss Config")]

    protected GameObject bossWidget;
    protected int hitIndex;
    [SerializeField] protected int numberOfHits;
    protected bool CanAttack;
    protected int currentWeaponActive;
    [SerializeField] protected IDestroyable[] DestroyableParts;

    protected BaseBossEnemyAI bossEnemyAI;

    [SerializeField]
    protected GameObject[] Weapons;
    [SerializeField]
    protected float delayAttak = 3;

    public override void InitReferences()
    {
        
        shipStatsSystem.SetStats(levelSystem);
        ShieldModuleInstalled = false;

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
            if (item.IsAlive == false)
            {
                return;
            }
        }

        BossTakeDamage();

        base.TakeDamage(damage);
    }

    public virtual void BossTakeDamage()
    {


        PlayerWeaponSystem playerWeaponSystem = GameObject.FindObjectOfType<PlayerWeaponSystem>();
        playerWeaponSystem.IncreasePowerUp(.05f);

        hitIndex++;

        if (hitIndex > numberOfHits)
        {
            hitIndex = 0;
            GetComponent<BossAI>().ChangeWaypoint(hitIndex);
        }
    }


    public override void Update()
    {
        base.Update();
        Tick();
    }

    public virtual void Tick()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Enter") || GuiManager.IsTrasnmiting())
        {
            return;
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
        {
            for (int i = 0; i < Weapons.Length; i++)
            {
                Weapons[i].SetActive(false);
            }
        }
        Attack();
    }

    public virtual void Phases()
    {
        for (int i = 0; i < Weapons.Length; i++)
        {
            if (Weapons[i].GetComponent<WeaponScript>().gameObject.activeSelf == false)
            {
                Weapons[i].GetComponent<WeaponScript>().gameObject.SetActive(true);
            }
        }

        if (shipStatsSystem.GetHealthPressentage() <= 50f)
        {
            //Debug.Log(shipStatsSystem.GetHealthPressentage());
            // Debug.Log("Increase Attack Speed");

            for (int i = 0; i < Weapons.Length; i++)
            {
                float newFireRate = Weapons[i].GetComponent<WeaponScript>().GetFireRate() - .2f;

                Weapons[i].GetComponent<WeaponScript>().SetFireRate(newFireRate);
            }
        }
    }

    public virtual void Attack()
    {
        if (delayAttak > 0)
        {
            delayAttak -= Time.deltaTime;
        }
        else
        {
            Phases();
        }
    }

    public override void Death()
    {
        base.Death();
    }
}
