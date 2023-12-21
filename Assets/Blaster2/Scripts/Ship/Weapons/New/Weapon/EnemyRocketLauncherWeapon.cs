using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRocketLauncherWeapon : BaseWeapon
{
   public override void Shoot()
    {
        StartCoroutine(RocketLauncherBlaster());
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
           BaseProjectile baseProjectile = InstansiatedProjectile.GetComponent<BaseProjectile>();
                baseProjectile.SetOwner(this);
                baseProjectile.SetShootDir(shootDir);

            }

            yield return new WaitForSeconds(1);
        }

        yield return null;
    }
}
