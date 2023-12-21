using UnityEngine;

public class RocketLauncherSpecialAttack : BaseSpecialAttack
{
    public override void Shoot()
    {
        if (SpecialActive)
        {
            if (Time.time > newShot)
            {
                newShot = Time.time + FireRate;

                InstansiatedProjectile = PoolManager.Instance.GetObjectFromPool(PoolGameObjectType.PlayerRocket);

                InstansiatedProjectile.SetActive(true);

                InstansiatedProjectile.transform.position = transform.position;

                BaseProjectile enemyProjectile = InstansiatedProjectile.GetComponent<BaseProjectile>();
                enemyProjectile.SetOwner(this);

                PlayWeaponFireSound();
            }
        }
    }


}