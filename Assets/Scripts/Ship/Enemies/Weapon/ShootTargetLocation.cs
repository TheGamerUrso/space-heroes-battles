using System.Collections;
using UnityEngine;

public class ShootTargetLocation : WeaponScript
{
    protected bool m_Shooting = false;
    public int m_NumberOfBullets;

    public override void Initialize()
    {
        if (GameObject.FindGameObjectWithTag("Player"))
        {
            direction = transform.position - GameObject.FindGameObjectWithTag("Player").transform.position;
        }
    }

    public override void Shoot()
    {
        if (!m_Shooting)
        {
            StartCoroutine(ShotTowardTargetCoroutine());
            m_Shooting = true;
        }
    }

    private IEnumerator ShotTowardTargetCoroutine()
    {
        int i = 0;

        for (i = 0; i < m_NumberOfBullets; i++)
        {
            InstansiatedProjectile = PoolManager.Instance.GetObjectFromPool(weaponData.m_Projectile);
            InstansiatedProjectile.transform.localPosition = Cannons[0].transform.position;
            InstansiatedProjectile.transform.rotation = Cannons[0].transform.rotation;
            InstansiatedProjectile.GetComponent<EnemyProjectile>().GetTargetLastPosition();
            InstansiatedProjectile.GetComponent<EnemyProjectile>().SetFollowTarget(true);
            InstansiatedProjectile.GetComponent<EnemyProjectile>().setDamage(weaponData.m_WeaponDamage);

            yield return new WaitForSeconds(1.0f);

            AudioManager.PlaySound(source, weaponData.ShootSoundEffect);

            i++;
        }

        yield return new WaitForSeconds(weaponData.m_FireRate);

        m_Shooting = false;
    }
}