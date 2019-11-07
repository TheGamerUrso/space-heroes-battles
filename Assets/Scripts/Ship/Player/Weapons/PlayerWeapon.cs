using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerWeapon : WeaponScript
{
    private PlayerAnimation playerAnimation;
    public Transform shipTransform;
    private bool holdFire;
    public void SetShipTransform(Transform shipTransform)
    {
        this.shipTransform = shipTransform;
    }

    public void SetPlayerAnimation(PlayerAnimation playerAnimation)
    {
        this.playerAnimation = playerAnimation;
    }

    public override void Update()
    {
        if (Time.frameCount % 1 == 0)
        {
            if (playerAnimation.GetAnimationState("Enter") || playerAnimation.GetAnimationState("Exit"))
            {
                return;
            }

            base.Update();
        }
    }

    public override void Shoot()
    {
        if (Input.touchCount > 0)
        {

            Fire();

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


    //if (Application.platform == RuntimePlatform.WindowsEditor)
    //{
    //    holdFire = Input.GetMouseButton(1) || CrossPlatformInputManager.GetButton("Fire2");

    //    if (!holdFire && Input.GetMouseButton(0) || CrossPlatformInputManager.GetButton("Fire1"))
    //    {
    //        Fire();
    //    }
    //}


    public override void Fire()
    {
        if (holdFire)
        {
            return;
        }

        if (Time.time > m_NewShot)
        {
            m_NewShot = Time.time + weaponData.m_FireRate;
            InstansiateBulletsByWeaponType(shipTransform.transform, ref weaponData.m_Projectile, ref weaponData.m_WeaponDamage);

            AudioManager.PlaySound(source, weaponData.ShootSoundEffect, 0, true);

            foreach (var item in particleSFX)
            {
                item.PlayEffect();
            }
        }
    }

    public void InstansiateBulletsByWeaponType(Transform ship, ref PoolGameObjectType m_Projectile, ref float m_WeaponDamage)
    {
        PlayerProjectile projectile = null;
        List<GameObject> Projectiles = PoolManager.Instance.GetPoolByType(PoolGameObjectType.PlayerProjectile);

        for (int i = 0; i < Cannons.Length; i++)
        {
            projectile = PoolManager.Instance.GetObjectFromPool(m_Projectile).GetComponent<PlayerProjectile>();
            projectile.transform.position = Cannons[i].transform.position;
            projectile.transform.rotation = Quaternion.Euler(new Vector3(0, ship.transform.eulerAngles.y, 0) + Cannons[i].eulerAngles);
            projectile.setDamage(weaponData.m_WeaponDamage + weaponData.multiplier);
        }
    }


}