using TheGamerUrso.PoolSystem;
using TheGamerUrso.PoolSystem;
using UnityEngine;

public class Blaster : WeaponScript
{
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
       if (Time.time > newShot)
        {
            newShot = Time.time + FireRate;

            PlayWeaponFireSound();

            for (int i = 0; i < Cannons.Length; i++)
            {
                InstansiatedProjectile = PoolManager.Instance.GetObjectFromPool(ProjectilePrefab);
                Vector3 shootDir = (Cannons[i].position - Cannons[i].forward).normalized;
                InstansiatedProjectile.transform.position = Cannons[i].position;
                InstansiatedProjectile.GetComponent<EnemyProjectile>().Setup(shootDir, Damage);
            }
        }
    }
 
}