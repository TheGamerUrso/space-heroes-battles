using System.Collections;
using UnityEngine;

public class RapidFireBlaster : Blaster
{
    private bool IsShooting;

    public override void Shoot()
    {
        if (!IsShooting)
        {
            timer = newShot - Time.time;

            if (timer < 1.5f)
            {
                AboutToShoot?.Invoke(true);
            }

            if (Time.time > newShot && AutoAttack)
            {
                StartCoroutine(ShootDelay());
            }
        }
    }

    IEnumerator ShootDelay()
    {
        IsShooting = true;
        for (int i = 0; i < Cannons.Length; i++)
        {

            if (!AutoAttack)
            {
                break;
            }

            if (source.isPlaying == false)
            {
                source.Play();
            }

            InstansiatedProjectile = PoolManager.Instance.GetObjectFromPool(ProjectilePrefab);
            dir = Cannons[i].position + Cannons[i].forward;
            shootDir = (dir - Cannons[i].position).normalized;
            InstansiatedProjectile.transform.position = Cannons[i].position;
            InstansiatedProjectile.transform.rotation = Quaternion.LookRotation(shootDir);
            InstansiatedProjectile.GetComponent<EnemyProjectile>().Setup(shootDir, weaponData.Damage);
            yield return new WaitForSeconds(weaponData.delayBetweenShots);
        }
        AboutToShoot?.Invoke(false);
        newShot = Time.time + weaponData.FireRate;
        IsShooting = false;
    }
}
