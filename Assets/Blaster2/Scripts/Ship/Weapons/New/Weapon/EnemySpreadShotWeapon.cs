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
            shootDir = ( dir - Cannons[i].position).normalized;

            InstansiatedProjectile.transform.position = Cannons[i].position;
            InstansiatedProjectile.transform.rotation = Quaternion.LookRotation(shootDir);

            BaseProjectile baseProjectile = InstansiatedProjectile.GetComponent<BaseProjectile>();
            baseProjectile.SetOwner(this);
            baseProjectile.SetShootDir(shootDir);

            InstansiatedProjectile.SetActive(true);
        }
        yield return null;
        IsShooting = false;
    }
}
