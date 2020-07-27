using UnityEngine;
using Random = UnityEngine.Random;

public class GrenadeLauncher : WeaponScript
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
            newShot = Time.time + FireRate;
            int pos = Random.Range(0, Cannons.Length);
            GameObject bomb = PoolManager.Instance.GetObjectFromPool(ProjectilePrefab);
            bomb.transform.position = Cannons[pos].transform.position;
            bomb.transform.rotation = Cannons[pos].rotation;

            Vector3 shootDir = Cannons[pos].forward;
            Projectile enemyProjectile = bomb.GetComponent<Projectile>();
            enemyProjectile.Setup(this);
            enemyProjectile.SetShootDir(shootDir);

            PlayWeaponFireSound();
        }
    }
}