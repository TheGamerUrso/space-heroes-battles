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

    public override void Initialize()
    {
        weaponData.m_FireRate = shipStatsSystem.FireRate;
    }

    private IEnumerator SpreadWeaponCoroutine()
    {
        int posToShoot = Radius;
        for (int i = 0; i < m_NumberOfBullets; i++)
        {
            GameObject newBullet = PoolManager.Instance.GetObjectFromPool(weaponData.m_Projectile);
            newBullet.transform.position = transform.position;
            newBullet.transform.rotation = Quaternion.Euler(new Vector3(0, posToShoot, 0));
            newBullet.GetComponent<Projectile>().setDamage(weaponData.m_WeaponDamage);
            posToShoot += Angle;
            yield return new WaitForSeconds(delayBetweenShots);
            AudioManager.PlaySound(source, weaponData.ShootSoundEffect);
        }
        yield return new WaitForSeconds(weaponData.m_FireRate);
        m_Shooting = false;
    }

}
