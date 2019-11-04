using UnityEngine;

public class MinerBoss : BaseBossEnemy
{
    public override void Enter()
    {
        currentWeaponActive = 1;

        Weapons[0].GetComponent<WeaponScript>().SetShipStatsSystem(shipStatsSystem);
    }
    public override void BossTakeDamage()
    {
        base.BossTakeDamage();

        if (Weapons[0].activeSelf == false)
        {
            Weapons[0].SetActive(true);
        }
    }

    public override void Attack()
    {
        if (baseEnemyAI.isEntered && baseEnemyAI.isAppeared)
        {
            if (delayAttak > 0)
            {
                delayAttak -= Time.deltaTime;
            }
            else
            {
                for (int i = 0; i < Weapons.Length; i++)
                {
                    Weapons[i].SetActive(true);
                }
            }
        }
    }

    public override void Death()
    {
        base.Death();

        for (int i = 0; i < currentWeaponActive; i++)
        {
            Weapons[i].SetActive(false);
        }

        EnemyProjectile[] enemyProjectiles = GameObject.FindObjectsOfType<EnemyProjectile>();

        foreach (EnemyProjectile item in enemyProjectiles)
        {
            item.gameObject.SetActive(false);
        }

        Destroy(gameObject, 2);
    }
}