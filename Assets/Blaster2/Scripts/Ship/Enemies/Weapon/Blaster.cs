using System;
using UnityEngine;

public class Blaster : WeaponScript
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
            newShot = Time.time + weaponData.FireRate;

            PlayWeaponFireSound();

            for (int i = 0; i < Cannons.Length; i++)
            {
                InstansiatedProjectile = PoolManager.Instance.GetObjectFromPool(ProjectilePrefab);
                dir = Cannons[i].position + Cannons[i].forward;
                shootDir = (dir - Cannons[i].position).normalized;
                InstansiatedProjectile.transform.position = Cannons[i].position;
                InstansiatedProjectile.transform.rotation = Quaternion.LookRotation(shootDir);
                InstansiatedProjectile.GetComponent<EnemyProjectile>().Setup(shootDir, weaponData.Damage);
            }
        }
    }
}