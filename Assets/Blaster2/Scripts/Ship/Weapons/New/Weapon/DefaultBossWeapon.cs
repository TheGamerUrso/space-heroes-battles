using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultBossWeapon : BaseWeapon
{
    public override void Shoot()
    {
        if (!IsShooting)
        {
            StartCoroutine(ShootDelay());
        }
    }

    IEnumerator ShootDelay()
    {
        IsShooting = true;
        for (int i = 0; i < Cannons.Length; i++)
        {

            if (!AutoAttack)
            {
                break;
            }

            if (source.isPlaying == false)
            {
                source.Play();
            }

            InstansiatedProjectile = PoolManager.Instance.GetObjectFromPool(ProjectilePrefab);
            dir = Cannons[i].position + Cannons[i].forward;
            shootDir = (dir - Cannons[i].position).normalized;

            InstansiatedProjectile.SetActive(true);

            InstansiatedProjectile.transform.position = Cannons[i].position;
            InstansiatedProjectile.transform.rotation = Quaternion.LookRotation(shootDir);

            BaseProjectile projectile = InstansiatedProjectile.GetComponent<BaseProjectile>();
            projectile.Setup(this);
            projectile.SetShootDir(shootDir);

            yield return new WaitForSeconds(weaponData.delayBetweenShots);
        }
        AboutToShoot?.Invoke(false);
        newShot = Time.time + FireRate;
        IsShooting = false;
    }
}
