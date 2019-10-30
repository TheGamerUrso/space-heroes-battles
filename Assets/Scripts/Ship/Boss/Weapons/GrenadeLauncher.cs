using UnityEngine;

public class GrenadeLauncher : Blaster
{

    public override void Fire()
    {
        if (Time.time > m_NewShot)
        {
            m_NewShot = Time.time + weaponData.m_FireRate;
            int pos = UnityEngine.Random.Range(0, Cannons.Length);
            GameObject bomb = PoolManager.Instance.GetObjectFromPool(weaponData.m_Projectile);
            bomb.transform.position = Cannons[pos].transform.position;
            bomb.transform.rotation = Cannons[pos].rotation;
            bomb.GetComponent<EnemyProjectile>().setDamage(weaponData.m_WeaponDamage);
            AudioManager.PlaySound(source, weaponData.ShootSoundEffect,0);
        }
    }
}