using System.Collections;
using TheGamerUrso.PoolSystem;
using UnityEngine;

public class ArtilleryWeapon : WeaponScript
{
    public float cooldown;
    private int numberOfAttacks;
    public bool attack;

    public override void OnUpdate()
    {
        base.OnUpdate();
        
        Shoot();

        if (!attack)
        {
            cooldown -= Time.deltaTime;
        }

        if (cooldown <= 0 && !attack)
        {
            cooldown = 2;
            numberOfAttacks = 0;
            attack = true;
        }

        if (numberOfAttacks > 3)
        {
            attack = false;
        }
    }


    public override void Shoot()
    {
        if (Time.time > newShot && AutoAttack && attack)
        {
            newShot = Time.time + FireRate;

            PlayWeaponFireSound();

            for (int i = 0; i < Cannons.Length; i++)
            {
                InstansiatedProjectile = PoolManager.Instance.GetObjectFromPool(ProjectilePrefab);
                Vector3 dir = Cannons[i].position + Cannons[i].up;
                Vector3 shootDir = (dir - Cannons[i].position).normalized;
                InstansiatedProjectile.transform.position = Cannons[i].position;
                InstansiatedProjectile.transform.rotation = Quaternion.LookRotation(shootDir);
                InstansiatedProjectile.GetComponent<EnemyProjectile>().Setup(shootDir, Damage);
            }

            numberOfAttacks++;
           
        }
    }
}

