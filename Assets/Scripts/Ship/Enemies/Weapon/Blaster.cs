using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
                InstansiatedProjectile.transform.position = Cannons[i].position;
                InstansiatedProjectile.transform.rotation = Cannons[i].rotation;
                InstansiatedProjectile.GetComponent<EnemyProjectile>().setDamage(Damage);
                InstansiatedProjectile.GetComponent<Rigidbody>().AddForce(InstansiatedProjectile.transform.forward * 100, ForceMode.Impulse);
            }
        }
    }
 
}