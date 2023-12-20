using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultEnemyWeapon : BaseWeapon
{
    public override void Shoot()
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
            enemyProjectile.Setup(this);
            enemyProjectile.SetShootDir(shootDir);
        }
    }
}
