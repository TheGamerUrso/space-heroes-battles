using System.Collections;
using UnityEngine;

public class SpreadWeapon : WeaponScript
{
    public bool m_Shooting;
    public int m_NumberOfBullets;

    private void OnEnable()
    {
        m_Shooting = false;
        if (ShootingCoroutine != null)
            StopCoroutine(ShootingCoroutine);
    }

    public override void Shoot()
    {
        if (!m_Shooting)
        {
            ShootingCoroutine = StartCoroutine(SpreadWeaponCoroutine());
        }
    }

    private IEnumerator SpreadWeaponCoroutine()
    {
        int posToShoot = weaponData.Radius;
        for (int i = 0; i < m_NumberOfBullets; i++)
        {
            InstansiatedProjectile = PoolManager.Instance.GetObjectFromPool(ProjectilePrefab);
            InstansiatedProjectile.SetActive(true);


            InstansiatedProjectile.transform.position = transform.position;
            InstansiatedProjectile.transform.rotation = Quaternion.Euler(new Vector3(0, posToShoot, 0));

            EnemyProjectile enemyProjectile = InstansiatedProjectile.GetComponent<EnemyProjectile>();
           // enemyProjectile.Setup(this);

            posToShoot += weaponData.Angle;
            yield return new WaitForSeconds(weaponData.delayBetweenShots);
            PlayWeaponFireSound();
        }
        yield return new WaitForSeconds(FireRate);
        m_Shooting = false;
    }

}
