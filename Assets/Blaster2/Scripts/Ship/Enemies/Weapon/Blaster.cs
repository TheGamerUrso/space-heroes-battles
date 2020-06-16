using System;
using TheGamerUrso.PoolSystem;
using UnityEngine;

public class Blaster : WeaponScript
{
    public Action<bool> AboutToShoot;
    public float timer;

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void OnUpdate()
    {
        Shoot();
    }

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

            PlayWeaponFireSound();

            for (int i = 0; i < Cannons.Length; i++)
            {
                InstansiatedProjectile = PoolManager.Instance.GetObjectFromPool(ProjectilePrefab);
                Vector3 dir = Cannons[i].position + Cannons[i].forward;
                Vector3 shootDir = (dir - Cannons[i].position).normalized;
                InstansiatedProjectile.transform.position = Cannons[i].position;
                InstansiatedProjectile.transform.rotation = Quaternion.LookRotation(shootDir);
                InstansiatedProjectile.GetComponent<EnemyProjectile>().Setup(shootDir, Damage);
            }
        }
    }

    public void OnDrawGizmos()
    {
        if (Cannons !=null && Cannons.Length > 0)
        {
            for (int i = 0; i < Cannons.Length; i++)
            {
                Gizmos.DrawLine(Cannons[i].position, Cannons[i].position + Cannons[i].forward);
                Vector3 shootDir = (Cannons[i].position - (Cannons[i].position + Cannons[i].forward)).normalized;
            }
        }
    }
}