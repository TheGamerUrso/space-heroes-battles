using System.Collections.Generic;
using TheGamerUrso.PoolSystem;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerWeapon : WeaponScript
{
    private PlayerAnimation playerAnimation;
    public Transform shipTransform;
    private bool holdFire;
    public float SuperChargeTime { get { return shipStatsSystem.SuperChargeTime; } set { shipStatsSystem.SuperChargeTime = value; } }
    public float SuperDamage { get { return shipStatsSystem.SuperDamage; } set { shipStatsSystem.SuperDamage = value; } }

    public int DamageMulitplier { get { return weaponData.multiplier; } }

    public bool HomeMissleUpgrade { get { return weaponData.m_HomeMissleUpgrade; } }

    public bool RapidFireMoade { get { return weaponData.RapidFireMode; } private set { } }

    protected bool autoAttack;
    public bool AutoAttack { get { return autoAttack; } set { autoAttack = value; } }

    protected bool usePitch;
    public void SetShipTransform(Transform shipTransform)
    {
        this.shipTransform = shipTransform;
    }

    public void SetPlayerAnimation(PlayerAnimation playerAnimation)
    {
        this.playerAnimation = playerAnimation;
    }
    public override void OnStart()
    {
        base.OnStart();
    }

    public override void Update()
    {
        
    }

    public override void OnUpdate()
    {
        base.OnUpdate(); 
        if (Time.frameCount % 1 == 0)
        {
            if (playerAnimation.GetAnimationState("Enter") || playerAnimation.GetAnimationState("Exit"))
            {
                return;
            }

            Shoot();
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

        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            holdFire = Input.GetMouseButton(1) || CrossPlatformInputManager.GetButton("Fire2");

            if (!holdFire && Input.GetMouseButton(0) || CrossPlatformInputManager.GetButton("Fire1"))
            {
                Fire();
            }
        }
    }




    public override void Fire()
    {
        if (holdFire)
        {
            return;
        }

        if (Time.time > newShot)
        {
            newShot = Time.time + GetFireRate();
            InstansiateBulletsByWeaponType(shipTransform.transform, ref weaponData.m_Projectile, ref weaponData.m_WeaponDamage);

            PlayWeaponFireSound(0,true);

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
            projectile.setDamage(Damage + DamageMulitplier);
        }
    }


}