using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;

public class DefaultPlayerWeapon : BaseWeapon
{
    protected GameObject foundPlayer;
    protected PlayerData playerData;
    protected PlayerShipData playerShipData;
    protected PlayerShip playerShip;
    protected bool holdFire;
    protected bool usePitch;

    public override void Awake()
    {
        base.Awake();
        foundPlayer = GameObject.FindGameObjectWithTag("Player");
        ship = foundPlayer.GetComponent<Ship>();
    }

    public override void Start()
    {
        base.Start();
        playerData = dataService.GetPlayerData();
        playerShipData = playerData.GetCurrentPlayerShipData();
        playerShip = ship.GetComponent<PlayerShip>();
    }

    public override void Update()
    {
        if (playerShip != null)
        {
            if (playerShip.GetAnimationState("Enter") || playerShip.GetAnimationState("Exit"))
            {
                return;
            }
        }

        var shouldShoot = false;

#if UNITY_STANDALONE || UNITY_EDITOR || UNITY_WEBGL
        shouldShoot = Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space) || Input.GetButton("Fire1");
        if (shouldShoot)
        {
            Shoot();
        }
#elif UNITY_ANDROID
        
        shouldShoot = Input.GetMouseButton(0) || Input.touchCount > 0;
        holdFire = Input.touchCount > 1 || (Input.GetMouseButton(1) && Input.GetMouseButton(0)) ? true : false;      
        if (shouldShoot && !holdFire)
        {  
            Shoot();
        }
#endif
    }


    public override void Shoot()
    {
        if (holdFire || !playerShip.CanFire)
        {
            return;
        }

        if (Time.time > newShot)
        {
            newShot = Time.time + FireRate;
            InstansiateBulletsByWeaponType();

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

    public void InstansiateBulletsByWeaponType()
    {
        for (int i = 0; i < Cannons.Length; i++)
        {
            InstansiatedProjectile = PoolManager.Instance.GetObjectFromPool(ProjectilePrefab);
            InstansiatedProjectile.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            Vector3 shootDir = Cannons[i].forward;
            InstansiatedProjectile.SetActive(true);

            InstansiatedProjectile.transform.position = Cannons[i].position;

            BaseProjectile projectile = InstansiatedProjectile.GetComponent<BaseProjectile>();
            projectile.SetOwner(this);
            projectile.SetShootDir(shootDir);
        }
    }


    public virtual void SetStats(PlayerShipData playerShipData, int weaponType = 1)
    {
        Damage = playerShipData.Damage / Cannons.Length;
        FireRate = playerShipData.FireRate;
    }


}