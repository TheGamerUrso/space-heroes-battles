using UnityEngine;

public class RocketLauncher : WeaponScript
{
    public override void Shoot()
    {
        if (HomeMissleUpgrade)
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
                if (Time.time > newShot)
                {
                    newShot = Time.time + FireRate;
                    GameObject rocket = PoolManager.Instance.GetObjectFromPool(PoolGameObjectType.PlayerRocket);
                    rocket.transform.position = transform.position;
                    /*
                    GameObject newBullet = Instantiate(weaponData.m_Projectile, middle,
                        Quaternion.Euler(new Vector3(0, 0, 0)));
                        */

                    rocket.GetComponent<Rocket>().setDamage(Damage);
                    rocket.GetComponent<Rocket>().HomeMissleType = false;
                    weaponData.m_NumberOfMissiles--;

                    PlayWeaponFireSound();
                }
            }
        }
    }

    public override void Initialize()
    {
        AutoAttack = true;
    }

}