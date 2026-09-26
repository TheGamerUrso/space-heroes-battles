using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRocketLauncherWeapon : EnemyWeapon
{
    public override void Shoot()
    {
        StartCoroutine(RocketLauncherBlaster());
    }

    IEnumerator RocketLauncherBlaster()
    {
        for (int repeat = 0; repeat < 2; repeat++)
        {
            for (int i = 0; i < Cannons.Length; i++)
            {
                float minRange = .8f;
                float maxRange = 1.2f;
                float prevPitch = source.pitch;
                source.pitch = UnityEngine.Random.Range(minRange, maxRange);

                if (source.isPlaying == false)
                {
                    source.Play();
                }

                if (weaponData.FollowTarget)
                {
                    playerLastLocation = Target.transform.position + (new Vector3(UnityEngine.Random.insideUnitCircle.x, 0, UnityEngine.Random.insideUnitCircle.y) * 2) - transform.position;
                    playerLastLocation.Normalize();
                }

                InstansiatedProjectile = PoolManager.Instance.GetObjectFromPool(weaponData.ProjectileType);
                InstansiatedProjectile.SetActive(true);

                dir = Cannons[i].position + Cannons[i].forward;
                shootDir = (dir - Cannons[i].position).normalized;

                InstansiatedProjectile.transform.position = Cannons[i].position;
                InstansiatedProjectile.transform.rotation = Quaternion.LookRotation(weaponData.FollowTarget ? playerLastLocation : shootDir);

                BaseProjectile projectile = InstansiatedProjectile.GetComponent<BaseProjectile>();
                projectile.SetOwner(this);
                projectile.SetShootDir(shootDir);

                if (weaponData.FollowTarget)
                    InstansiatedProjectile.GetComponent<Rigidbody>().AddForce(playerLastLocation * 100, ForceMode.Impulse);
            }
        }
        AboutToShoot?.Invoke(false);
        delayAttackTimer = weaponData.DelayBetweenShots;
        IsShooting = false;
        yield return null;
    }
}
