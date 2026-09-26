using UnityEngine;

public class RocketLauncherSpecialAttack : BaseSpecialAttack
{
    public override void Shoot()
    {
        if (SpecialActive)
        {
            if (Time.time > nextShot)
            {
                nextShot = Time.time + FireRate;

                float minRange = .8f;
                float maxRange = 1.2f;
                float prevPitch = source.pitch;
                source.pitch = UnityEngine.Random.Range(minRange, maxRange);

                if (source.isPlaying == false)
                {
                    source.Play();
                }

                InstansiatedProjectile = PoolManager.Instance.GetObjectFromPool(weaponData.ProjectileType);
                InstansiatedProjectile.transform.position = transform.position;
                InstansiatedProjectile.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                Vector3 shootDir = transform.forward;
                InstansiatedProjectile.SetActive(true);

                BaseProjectile projectile = InstansiatedProjectile.GetComponent<BaseProjectile>();
                projectile.SetOwner(this);
                projectile.SetShootDir(shootDir);
            }
        }
    }


}