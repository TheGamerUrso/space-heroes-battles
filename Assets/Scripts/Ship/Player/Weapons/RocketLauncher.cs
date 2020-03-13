using TheGamerUrso.PoolSystem;
using UnityEngine;
using TheGamerUrso;

public class RocketLauncher : PlayerWeapon
{
    public override void Shoot()
    {
        if (HomeMissleUpgrade)
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

                        rocket.GetComponent<Rocket>().Setup(transform.forward, SuperDamage);
                        rocket.GetComponent<Rocket>().HomeMissleType = false;
                        weaponData.m_NumberOfMissiles--;
                        PlayWeaponFireSound();
                    }
                }
            }
        }
    }

    public override void Initialize()
    {
        AutoAttack = true;
    }

}