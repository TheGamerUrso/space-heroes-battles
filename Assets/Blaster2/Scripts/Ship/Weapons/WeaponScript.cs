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
    protected Ship ship;
    public WeaponData weaponData;
    [SerializeField] protected AudioSource source;

    [Header("Weapon")]
    [SerializeField]
    protected List<WeaponFireEffect> particleSFX = new List<WeaponFireEffect>();
    [SerializeField] protected Transform[] Cannons;
   
    protected int Radius;
    protected int Angle;


    protected Coroutine ShootingCoroutine;
    [SerializeField] protected float delayBetweenShots;
    protected GameObject InstansiatedProjectile;
    [SerializeField] protected bool autoAttack;
    protected float newShot;

    #endregion

    #region WeaponData Getters

    public float fireRate;
    public float damage;
    public float FireRate { get { return fireRate; } set { fireRate = value; } }

    public float Damage { get { return damage; } set { damage = value; } }

    public AudioClip SoundSFX { get { return weaponData.ShootSoundEffect; } private set { } }

    public PoolGameObjectType ProjectilePrefab { get { return weaponData.m_Projectile; } private set { } }


    public bool AutoAttack
    {
        get
        {
            return autoAttack;
        }

        set
        {
            autoAttack = value;
        }
    }
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
        if (ship != null)
        {
            fireRate = ship.FireRate;
            damage = ship.Damage;
        }
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
        return FireRate;
    }

    public virtual void SetFireRate(float fireRate)
    {
        this.fireRate = fireRate;
    }

    #endregion

    public void PlayWeaponFireSound(bool usePitch = false, float minRange = .8f, float maxRange = 1.2f)
    {
        if (usePitch)
        {
            float prevPitch = source.pitch;
            source.pitch = UnityEngine.Random.Range(minRange, maxRange);
        }
        source.PlayOneShot(SoundSFX);
    }


}