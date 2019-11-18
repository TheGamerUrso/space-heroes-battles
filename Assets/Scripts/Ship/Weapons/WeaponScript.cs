using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public abstract class WeaponScript : MonoBehaviour
{

    #region Weapon Variables
    [Header("Weapon")]
    public Action<WeaponScript> onUpdate;
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

    protected float newShot;
    protected ShipStatsSystem shipStatsSystem;
    protected Ship ship;

    protected bool usePitch;
    protected bool autoAttack;
    #endregion

    #region WeaponData Getters
    public int DamageMulitplier { get { return weaponData.multiplier; } }

    public bool HomeMissleUpgrade { get { return weaponData.m_HomeMissleUpgrade; } }
    public bool RapidFireMoade { get { return weaponData.RapidFireMode; } private set { } }

    public bool AutoAttack { get { return autoAttack; } set { autoAttack = value; } }

    public float SuperChargeTime { get { return shipStatsSystem.SuperChargeTime; } set { shipStatsSystem.SuperChargeTime = value; } }
    public float SuperDamage { get { return shipStatsSystem.SuperDamage; } set { shipStatsSystem.SuperDamage = value; } }

    public float FireRate { get { return shipStatsSystem.FireRate; } set { shipStatsSystem.FireRate = value; } }
    public float Damage { get { return shipStatsSystem.Damage; } set { shipStatsSystem.Damage = value; } }
    public AudioClip SoundSFX { get { return weaponData.ShootSoundEffect; } private set { } }
    public PoolGameObjectType ProjectilePrefab { get { return weaponData.m_Projectile; } private set { } }
    #endregion


    public void SetShipStatsSystem(ShipStatsSystem shipStatsSystem)
    {
        this.shipStatsSystem = shipStatsSystem;
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

    public virtual void Initialize() { }

    public virtual void Fire() { }

    public abstract void Shoot();

    public virtual void SetDamage(float damage)
    {
        shipStatsSystem.Damage = damage;
    }

    public float GetFireRate()
    {
        return shipStatsSystem.FireRate;
    }

    public virtual void SetFireRate(float fireRate)
    {
        if (shipStatsSystem == null)
        {
            Debug.LogError (gameObject.name + " shipStatsSystem is null");
        }
        shipStatsSystem.FireRate = fireRate;
    }

    public void PlayWeaponFireSound(int audioMixGroup = 0,bool usePitch = false)
    {
        AudioManager.PlaySound(source, SoundSFX, audioMixGroup, usePitch);
    }

    public void SetShip(Ship ship)
    {
        this.ship = ship;
    }
}