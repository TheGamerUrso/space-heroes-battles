using System.Collections;
using TheGamerUrso.PoolSystem;
using UnityEngine;

public class ShootTargetLocation : Blaster
{
    public GameObject Target;
    public Vector3 playerLastLocation;
    public override void Initialize()
    {
        base.Initialize();
    }
    public override void OnStart()
    {
        base.OnStart();

        if (Target == null)
        {
            Target = GameObject.FindGameObjectWithTag("Player");
        }
    }
    public override void OnUpdate()
    {
        base.OnUpdate();

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
                playerLastLocation = Target.transform.position + (new Vector3(Random.insideUnitCircle.x, 0, Random.insideUnitCircle.y) * 2) - transform.position;
                playerLastLocation.Normalize();
                InstansiatedProjectile = PoolManager.Instance.GetObjectFromPool(ProjectilePrefab);
                InstansiatedProjectile.transform.position = Cannons[i].position;
                InstansiatedProjectile.transform.rotation = Quaternion.LookRotation(playerLastLocation);
                InstansiatedProjectile.GetComponent<EnemyProjectile>().Damage = Damage;

                //InstansiatedProjectile.GetComponent<Rigidbody>().AddForce(InstansiatedProjectile.transform.forward * 100, ForceMode.Impulse);

                InstansiatedProjectile.GetComponent<Rigidbody>().AddForce(playerLastLocation * 100, ForceMode.Impulse);

            }
        }

    }
}