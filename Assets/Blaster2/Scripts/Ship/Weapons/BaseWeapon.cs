using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public abstract class BaseWeapon : MonoBehaviour
{
    protected Ship ship;
    public Action<bool> AboutToShoot;
    public Weapon_SO weaponData;

    public float FireRate { get; set; }
    public float Damage { get; set; }
    public float SuperChargeTime { get; set; }


    [SerializeField] protected AudioSource source;
    [SerializeField] protected Transform[] Cannons;
    [SerializeField]
    protected List<WeaponFireEffect> particleSFX = new List<WeaponFireEffect>();

    protected Coroutine ShootingCoroutine;
    protected GameObject InstansiatedProjectile;

    protected Vector3 dir;
    protected Vector3 shootDir;
    protected float timer;
    protected bool IsShooting;
    protected float nextShot;
    protected float delayAttackTimer = 0;


    protected GameObject Target;
    protected Vector3 playerLastLocation;
    public bool AutoAttack { get; set; }

    public virtual void Initialize(Ship ship, float damage, float fireRate)
    {
        this.ship = ship;
        Cannons = transform.Cast<Transform>().ToArray();
        SetStats(damage, fireRate);
    }

    public virtual void Update()
    {
        if (IsShooting)
        {
            delayAttackTimer -= Time.deltaTime;
            if (delayAttackTimer <= 0)
            {
                timer = nextShot - Time.time;

                if (timer < 1.5f)
                {
                    AboutToShoot?.Invoke(true);
                }
            }
        }
        else if (!IsShooting)
        {
            if (Time.time > nextShot && AutoAttack)
            {
                IsShooting = true;
                delayAttackTimer = weaponData.DelayBetweenShots;
                AboutToShoot?.Invoke(false);
                nextShot = Time.time + FireRate;
                Shoot();
            }
        }
    }

    public abstract void Shoot();

    public virtual void SetStats(float damage,float fireRate)
    {
        Damage = damage / Cannons.Length;
        FireRate = fireRate;
    }
}
