using System;
using System.Collections.Generic;
using System.Linq;
using TheGamerUrso.PoolSystem;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public abstract class WeaponScript : MonoBehaviour
{
    #region Weapon Variables
    [Header("Weapon")]
    [SerializeField]
    protected List<WeaponFireEffect> particleSFX = new List<WeaponFireEffect>();
    [SerializeField] protected Transform[] Cannons;
    public WeaponData weaponData;
    protected int Radius;
    protected int Angle;
    [SerializeField] protected AudioSource source;
    [SerializeField] protected bool FollowRotation;
    protected Vector3 direction;
    protected Coroutine ShootingCoroutine;
    [SerializeField] protected float delayBetweenShots;
    protected GameObject InstansiatedProjectile;

    protected float newShot;
    protected ShipStatsSystem shipStatsSystem;
    protected Ship ship;
    #endregion

    #region WeaponData Getters
    public float FireRate { get { return shipStatsSystem.FireRate; } set { shipStatsSystem.FireRate = value; } }

    public float Damage { get { return shipStatsSystem.Damage; } set { shipStatsSystem.Damage = value; } }

    public AudioClip SoundSFX { get { return weaponData.ShootSoundEffect; } private set { } }

    public PoolGameObjectType ProjectilePrefab { get { return weaponData.m_Projectile; } private set { } }
    #endregion


    private void Start()
    {
        OnStart();
        Initialize();
    }

    public virtual void Update()
    {
        OnUpdate();
    }
    public virtual void OnStart()
    {
        source = GetComponent<AudioSource>();
        Cannons = transform.Cast<Transform>().ToArray();
        ship = GetComponentInParent<Ship>();
        shipStatsSystem = ship.GetShipStatsSystem();
    }

    public virtual void OnUpdate() { }

    public virtual void Initialize() { }

    public virtual void Fire() { }

    public abstract void Shoot();

    #region Getters and Setters

    public void SetShip(Ship ship)
    {
        this.ship = ship;
    }

    public float GetDamage()
    {
        return Damage;
    }

    public void SetDamage(float damage)
    {
        Damage = damage;
    }
    public float GetFireRate()
    {
        return shipStatsSystem.FireRate;
    }

    public virtual void SetFireRate(float fireRate)
    {
        if (shipStatsSystem == null)
        {
            Debug.LogError(gameObject.name + " shipStatsSystem is null");
        }
        shipStatsSystem.FireRate = fireRate;
    }
    #endregion

    public void PlayWeaponFireSound(int audioMixGroup = 0, bool usePitch = false)
    {
        if (AudioManager.Instance)
            AudioManager.PlaySound(source, SoundSFX, audioMixGroup, usePitch);
    }


}