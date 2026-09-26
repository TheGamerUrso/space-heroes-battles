using UnityEngine;
using Random = UnityEngine.Random;

public class GrenadeLauncherWeapon : BaseWeapon
{
    public override void Shoot()
    {
        int pos = Random.Range(0, Cannons.Length);

        InstansiatedProjectile = PoolManager.Instance.GetObjectFromPool(weaponData.ProjectileType);
        InstansiatedProjectile.transform.position = Cannons[pos].transform.position;
        InstansiatedProjectile.transform.rotation = Cannons[pos].rotation;

        Vector3 shootDir = Cannons[pos].forward;

        InstansiatedProjectile.SetActive(true);

        BaseProjectile enemyProjectile = InstansiatedProjectile.GetComponent<BaseProjectile>();
        enemyProjectile.SetDamage(Damage);
        enemyProjectile.SetShootDir(shootDir);
    }
}