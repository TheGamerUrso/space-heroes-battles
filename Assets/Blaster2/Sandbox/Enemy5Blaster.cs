using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy5Blaster : Blaster
{
    public override void Shoot()
    {
        timer = newShot - Time.time;

        if (timer < 1.5f)
        {
            AboutToShoot?.Invoke(true);
        }

        if (Time.time > newShot && AutoAttack)
        {
            AboutToShoot?.Invoke(false);
            newShot = Time.time + FireRate;

            StartCoroutine(RocketLauncherBlaster());
        }
    }

    IEnumerator RocketLauncherBlaster()
    {

        for (int repeat = 0; repeat < 2; repeat++)
        {
            PlayWeaponFireSound();

            for (int i = 0; i < Cannons.Length; i++)
            {          
                InstansiatedProjectile = PoolManager.Instance.GetObjectFromPool(ProjectilePrefab);
                dir = Cannons[i].position + Cannons[i].forward;
                shootDir = (dir - Cannons[i].position).normalized;
                InstansiatedProjectile.SetActive(true);
                InstansiatedProjectile.transform.position = Cannons[i].position;
                InstansiatedProjectile.transform.rotation = Quaternion.LookRotation(shootDir);

                EnemyProjectile enemyProjectile = InstansiatedProjectile.GetComponent<EnemyProjectile>();
                //enemyProjectile.Setup(this);
                enemyProjectile.SetShootDir(shootDir);

            }

            yield return new WaitForSeconds(1);
        }

        yield return null;
    }
}
