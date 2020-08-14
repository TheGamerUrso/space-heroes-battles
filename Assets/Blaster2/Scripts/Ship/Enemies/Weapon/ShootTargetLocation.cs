using System.Collections;
using UnityEngine;

public class ShootTargetLocation : WeaponScript
{
    [SerializeField] private GameObject Target;
    private Vector3 playerLastLocation;

    public override void Start()
    {
        base.Start();

        if (Target == null)
        {
            Target = GameObject.FindGameObjectWithTag("Player");
        }
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
                InstansiatedProjectile.SetActive(true);

                InstansiatedProjectile.transform.position = Cannons[i].position;
                InstansiatedProjectile.transform.rotation = Quaternion.LookRotation(playerLastLocation);

                EnemyProjectile enemyProjectile = InstansiatedProjectile.GetComponent<EnemyProjectile>();
                enemyProjectile.Setup(this);
                enemyProjectile.SetShootDir(playerLastLocation);


                InstansiatedProjectile.GetComponent<Rigidbody>().AddForce(playerLastLocation * 100, ForceMode.Impulse);
            }
        }

    }
}