using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDoubleShotWeapon : BaseWeapon
{
     public override void Shoot()
    {
       StartCoroutine(DoubleShot());
    }

    IEnumerator DoubleShot()
    {
        for (int X = 0; X < 2; X++)
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
                enemyProjectile.SetOwner(this);
                enemyProjectile.SetShootDir(shootDir);
            }

            yield return new WaitForSeconds(.5f);
        }
    }
}
