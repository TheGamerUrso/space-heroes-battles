using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public abstract class WeaponScript : MonoBehaviour
{
    [Header("Weapon")]
    public  Action<WeaponScript>onUpdate;
    [SerializeField]
    protected List<WeaponFireEffect> particleSFX = new List<WeaponFireEffect>();
    protected Transform[] Cannons;
    public WeaponData weaponData;
    protected int Radius;
    protected int Angle;
    [SerializeField] protected AudioSource source;
    [SerializeField] protected bool FollowRotation;
    protected Vector3 direction;
    protected Coroutine ShootingCoroutine;
    [SerializeField] protected float delayBetweenShots;
    protected GameObject InstansiatedProjectile;

    protected float m_NewShot;
    protected ShipStatsSystem shipStatsSystem;
    protected Ship ship;


    public void SetShip(Ship ship)
    {
        this.ship = ship;
    }

    public void SetShipStatsSystem(ShipStatsSystem shipStatsSystem)
    {
        this.shipStatsSystem = shipStatsSystem;
        InitWeapon();
    }

    private void OnValidate()
    {
        particleSFX = new List<WeaponFireEffect>();
        foreach (Transform item in transform)
        {
            particleSFX.Add(item.GetComponentInChildren<WeaponFireEffect>());
        }
    }

    private void Start()
    {
        source = GetComponent<AudioSource>();
        
        Cannons = transform.Cast<Transform>().ToArray();
        Initialize();
    }

    public virtual void Update()
    {
        if (onUpdate != null) onUpdate(this);


        if (weaponData.CanAttack)
        {
            Shoot();
        }
    }

    public virtual void InitWeapon()
    {
        weaponData.m_FireRate = shipStatsSystem.FireRate;
        weaponData.m_WeaponDamage = shipStatsSystem.Damage;
    }

    public virtual void Initialize(){}

    public virtual void Fire() { }

    public abstract void Shoot();

    public virtual void SetDamage(float damage)
    {
        weaponData.m_WeaponDamage = damage;
    }

    public float GetFireRate()
    {
        return weaponData.m_FireRate;
    }

    public virtual void SetFireRate(float fireRate)
    {
        weaponData.m_FireRate = fireRate;
    }
}