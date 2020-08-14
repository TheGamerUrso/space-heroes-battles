using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy6Blaster : Blaster
{
    private bool shooting;

    public override void Shoot()
    {
        if (!shooting)
        {
            timer = newShot - Time.time;

            if (timer < 1.5f)
            {
                AboutToShoot?.Invoke(true);
            }
        }

        if (Time.time > newShot && AutoAttack)
        {
            AboutToShoot?.Invoke(false);
            newShot = Time.time + FireRate;

            PlayWeaponFireSound();

            StartCoroutine(BlasterSpreadShot());
        }
    }

    IEnumerator BlasterSpreadShot()
    {
        shooting = true;
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
        shooting = false;
    }
}
