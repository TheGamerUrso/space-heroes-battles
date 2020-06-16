using System.Collections.Generic;
using TheGamerUrso.PoolSystem;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerWeapon : WeaponScript
{
    public Transform shipTransform;
    private bool holdFire;

    public int DamageMulitplier { get { return weaponData.multiplier; } }


    protected bool usePitch;

    public void SetShipTransform(Transform shipTransform)
    {
        this.shipTransform = shipTransform;
    }

    public override void OnStart()
    {
        base.OnStart();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (Time.frameCount % 1 == 0)
        {
            if (ship != null)
            {
                PlayerShip playerShip = ship.GetComponent<PlayerShip>();


                if (playerShip != null)
                {
                    if (playerShip.GetAnimationState("Enter") || playerShip.GetAnimationState("Exit"))
                    {
                        return;
                    }
                }
            }
        

            if (Input.touchCount > 0)
            {
                if (Input.touchCount > 1)
                {
                    holdFire = true;
                }
                else
                {
                    holdFire = false;
                }

      

                Shoot();

            }

            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                holdFire = Input.GetMouseButton(1) && Input.GetMouseButton(0);

                if (!holdFire && Input.GetMouseButton(0))
                {
                    Shoot();
                }
            }
        }
    }

    public override void Shoot()
    {

        if (holdFire || !GameSession.CanFire)
        {
            return;
        }

        if (Time.time > newShot)
        {
            newShot = Time.time + GetFireRate();
            InstansiateBulletsByWeaponType(shipTransform.transform, ref weaponData.m_Projectile, ref weaponData.m_WeaponDamage);

            PlayWeaponFireSound(true);

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

    public void InstansiateBulletsByWeaponType(Transform ship, ref PoolGameObjectType m_Projectile, ref float m_WeaponDamage)
    {
        List<GameObject> Projectiles = PoolManager.Instance.GetPoolByType(PoolGameObjectType.PlayerProjectile);

        for (int i = 0; i < Cannons.Length; i++)
        {
            InstansiatedProjectile = PoolManager.Instance.GetObjectFromPool(ProjectilePrefab);
            InstansiatedProjectile.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0))  ;

            Vector3 shootDir = Cannons[i].forward;
            InstansiatedProjectile.transform.position = Cannons[i].position;
            InstansiatedProjectile.GetComponent<PlayerProjectile>().Setup(shootDir, Damage + DamageMulitplier);
        }
    }


}