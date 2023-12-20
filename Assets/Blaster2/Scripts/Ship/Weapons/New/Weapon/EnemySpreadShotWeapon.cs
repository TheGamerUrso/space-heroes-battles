using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpreadShotWeapon : BaseWeapon
{
    public override void Shoot()
    {
        PlayWeaponFireSound();
        StartCoroutine(BlasterSpreadShot());
    }

    IEnumerator BlasterSpreadShot()
    {
        IsShooting = true;
        for (int i = 0; i < Cannons.Length; i++)
        {
            InstansiatedProjectile = PoolManager.Instance.GetObjectFromPool(ProjectilePrefab);
            dir = Cannons[i].position + Cannons[i].forward;
            shootDir = (Cannons[i].position - dir).normalized;

            InstansiatedProjectile.transform.position = Cannons[i].position;
            InstansiatedProjectile.transform.rotation = Quaternion.LookRotation(shootDir);

            EnemyProjectile enemyProjectile = InstansiatedProjectile.GetComponent<EnemyProjectile>();
            enemyProjectile.Setup(this);
            enemyProjectile.SetShootDir(shootDir);

            InstansiatedProjectile.SetActive(true);
        }
        yield return null;
        IsShooting = false;
    }
}
