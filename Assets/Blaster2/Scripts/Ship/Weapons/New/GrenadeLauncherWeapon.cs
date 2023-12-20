using UnityEngine;
using Random = UnityEngine.Random;

public class GrenadeLauncherWeapon : BaseWeapon
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

            InstansiatedProjectile = PoolManager.Instance.GetObjectFromPool(ProjectilePrefab);
            InstansiatedProjectile.transform.position = Cannons[pos].transform.position;
            InstansiatedProjectile.transform.rotation = Cannons[pos].rotation;

            Vector3 shootDir = Cannons[pos].forward;

            InstansiatedProjectile.SetActive(true);

            BaseProjectile enemyProjectile = InstansiatedProjectile.GetComponent<BaseProjectile>();
            enemyProjectile.Setup(this);
            enemyProjectile.SetShootDir(shootDir);

            PlayWeaponFireSound();
        }
    }
}