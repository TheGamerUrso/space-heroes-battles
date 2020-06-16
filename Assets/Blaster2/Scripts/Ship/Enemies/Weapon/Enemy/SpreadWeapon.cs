using System.Collections;
using TheGamerUrso.PoolSystem;
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
        int posToShoot = Radius;
        for (int i = 0; i < m_NumberOfBullets; i++)
        {
            GameObject newBullet = PoolManager.Instance.GetObjectFromPool(ProjectilePrefab);
            newBullet.transform.position = transform.position;
            newBullet.transform.rotation = Quaternion.Euler(new Vector3(0, posToShoot, 0));
            newBullet.GetComponent<Projectile>().Damage = Damage;
            posToShoot += Angle;
            yield return new WaitForSeconds(delayBetweenShots);
            PlayWeaponFireSound();
        }
        yield return new WaitForSeconds(GetFireRate());
        m_Shooting = false;
    }

}
