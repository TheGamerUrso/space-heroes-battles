using System.Collections;
using TheGamerUrso.PoolSystem;
using UnityEngine;

public class BurstWeapon : Blaster
{
    public int repeat;
    public bool m_Shooting;

    public override void Shoot()
    {
        if (!m_Shooting)
        {
            StartCoroutine(BurstWeaponCoroutine());
            m_Shooting = true;
        }
    }

    private IEnumerator BurstWeaponCoroutine()
    {
        int posToShoot = Radius;
        for (int i = 0; i < m_NumberOfBullets; i++)
        {
            GameObject newBullet = PoolManager.Instance.GetObjectFromPool(PoolGameObjectType.EnemyProjectile);
            newBullet.transform.position = transform.position;
            newBullet.transform.rotation = Quaternion.Euler(new Vector3(0, Random.Range(-40, 40), 0));
            newBullet.GetComponent<Projectile>().setDamage(Damage);
            posToShoot += Angle;
            yield return new WaitForSeconds(.1f);
        }
        yield return new WaitForSeconds(FireRate);
        m_Shooting = false;
    }
}