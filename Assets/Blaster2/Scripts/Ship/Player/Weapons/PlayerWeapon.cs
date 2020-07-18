using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : WeaponScript
{
    protected GameObject foundPlayer;
    protected PlayerData playerData;
    protected PlayerShipData playerShipData;
    protected PlayerShip playerShip;
    protected bool holdFire;
    protected bool usePitch;

    public override void Awake()
    {
        foundPlayer = GameObject.FindGameObjectWithTag("Player");
        ship = foundPlayer.GetComponent<Ship>();
    }

    public override void Start()
    {
        base.Start();
        playerData = PersistantData.GetPlayerData();
        playerShipData = playerData.GetCurrentPlayerShipData();
        playerShip = ship.GetComponent<PlayerShip>();
    }

    public override void Update()
    {
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


            holdFire = Input.GetMouseButton(1) && Input.GetMouseButton(0);

            if (!holdFire && Input.GetMouseButton(0))
            {
                Shoot();
            }
        }
    }


    public override void Shoot()
    {
        if (holdFire || !playerShip.CanFire)
        {
            return;
        }

        if (Time.time > newShot)
        {
            newShot = Time.time + weaponData.FireRate;
            InstansiateBulletsByWeaponType(foundPlayer.transform, ref weaponData.m_Projectile);

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

    public void InstansiateBulletsByWeaponType(Transform ship, ref PoolGameObjectType m_Projectile)
    {
        for (int i = 0; i < Cannons.Length; i++)
        {
            InstansiatedProjectile = PoolManager.Instance.GetObjectFromPool(ProjectilePrefab);
            InstansiatedProjectile.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

            Vector3 shootDir = Cannons[i].forward;
            InstansiatedProjectile.transform.position = Cannons[i].position;
            InstansiatedProjectile.GetComponent<PlayerProjectile>().Setup(shootDir, weaponData.Damage);
        }
    }


}