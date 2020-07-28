using UnityEngine;

public class RocketLauncher : SpecialAttack
{
    public override void Shoot()
    {
        if (SpecialActive)
        {
            if (Time.time > newShot)
            {
                newShot = Time.time + FireRate;

                InstansiatedProjectile =
                    PoolManager.Instance.GetObjectFromPool(PoolGameObjectType.PlayerRocket);

                InstansiatedProjectile.transform.position = transform.position;

                Projectile enemyProjectile = InstansiatedProjectile.GetComponent<Projectile>();
                enemyProjectile.Setup(this);

                PlayWeaponFireSound();

            }
        }
    }


}