using UnityEngine;

public class RocketLauncher : SpecialAttack
{
    public override void Shoot()
    {
        if (SpecialActive)
        {
            if (Time.time > newShot)
            {
                newShot = Time.time + weaponData.FireRate;
                GameObject rocket =
                    PoolManager.Instance.GetObjectFromPool(PoolGameObjectType.PlayerRocket);

                rocket.transform.position = transform.position;

                rocket.GetComponent<Rocket>().Damage = weaponData.Damage;

                PlayWeaponFireSound();

            }
        }
    }


}