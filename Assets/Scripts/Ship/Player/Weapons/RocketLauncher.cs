using UnityEngine;

public class RocketLauncher : WeaponScript
{
    public override void Shoot()
    {
        if (weaponData.m_HomeMissleUpgrade)
        {
            HomeMissle();
        }
    }

    public void HomeMissle()
    {
        var middle = transform.position + new Vector3(0, 0, 1);
        if (weaponData.m_HomeMissleUpgrade &&
            weaponData.m_NumberOfMissiles > 0 &&
            !weaponData.m_RocketUpgrade)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (Time.time > m_NewShot)
                {
                    m_NewShot = Time.time + weaponData.m_FireRate;
                    GameObject rocket = PoolManager.Instance.GetObjectFromPool(PoolGameObjectType.PlayerRocket);
                    rocket.transform.position = transform.position;
                    /*
                    GameObject newBullet = Instantiate(weaponData.m_Projectile, middle,
                        Quaternion.Euler(new Vector3(0, 0, 0)));
                        */

                    rocket.GetComponent<Rocket>().setDamage(weaponData.m_WeaponDamage);
                    rocket.GetComponent<Rocket>().HomeMissleType = false;
                    weaponData.m_NumberOfMissiles--;

                    AudioManager.PlaySound(source, weaponData.ShootSoundEffect);
                }
            }
        }
    }


    public override void InitWeapon()
    {
        weaponData.m_WeaponDamage = shipStatsSystem.SuperDamage;
        weaponData.m_FireRate = shipStatsSystem.FireRate;
        weaponData.AutoAttack = true;
    }
}