using UnityEngine;

public class PlayerWeapon : BaseWeapon
{
    protected PlayerData playerData;
    protected PlayerShipData playerShipData;
    protected bool holdFire;
    protected bool usePitch;
    protected bool CanFire;

    public override void Shoot()
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

            for (int i = 0; i < Cannons.Length; i++)
            {
                InstansiatedProjectile = PoolManager.Instance.GetObjectFromPool(weaponData.ProjectileType);
                InstansiatedProjectile.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                Vector3 shootDir = Cannons[i].forward;
                InstansiatedProjectile.SetActive(true);

                InstansiatedProjectile.transform.position = Cannons[i].position;

                BaseProjectile projectile = InstansiatedProjectile.GetComponent<BaseProjectile>();
                projectile.SetOwner(this);
                projectile.SetShootDir(shootDir);
            }         

            foreach (var item in particleSFX)
            {
                item.PlayEffect();
            }
        }

        if (Input.touchCount > 1)
        {
            holdFire = true;
        }
        else
        {
            holdFire = false;
        }
    }
}