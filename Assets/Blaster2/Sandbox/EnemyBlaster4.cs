using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBlaster4 : Blaster
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
            
            StartCoroutine(DoubleShot());
        }
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
                //enemyProjectile.Setup(this);
                enemyProjectile.SetShootDir(shootDir);
            }

            yield return new WaitForSeconds(.5f);
        }
    }
}
