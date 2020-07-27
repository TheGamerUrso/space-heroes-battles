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

                GameObject rocket =
                    PoolManager.Instance.GetObjectFromPool(PoolGameObjectType.PlayerRocket);

                rocket.transform.position = transform.position;

                EnemyProjectile enemyProjectile = InstansiatedProjectile.GetComponent<EnemyProjectile>();
                enemyProjectile.Setup(this);

                PlayWeaponFireSound();

            }
        }
    }


}