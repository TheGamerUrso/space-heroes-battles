using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    public Action<bool> AboutToShoot;

    [SerializeField] protected Ship ship;
    public Weapon_SO weaponData;

    public float FireRate;
    public float Damage;
    public float SuperChargeTime;

    [SerializeField] protected AudioSource source;

    [SerializeField]
    protected List<WeaponFireEffect> particleSFX = new List<WeaponFireEffect>();
    [SerializeField] protected Transform[] Cannons;

    protected Coroutine ShootingCoroutine;
    protected GameObject InstansiatedProjectile;
    protected float newShot;

    protected Vector3 dir;
    protected Vector3 shootDir;
    protected float timer;
    protected bool IsShooting;
    public bool AutoAttack { get; set; }


    public AudioClip SoundSFX { get { return weaponData.ShootSFX; } }
    public PoolGameObjectType ProjectilePrefab { get { return weaponData.m_Projectile; } }

    public virtual void Awake() { }
    public virtual void Start()
    {
        source = GetComponent<AudioSource>();
        Cannons = transform.Cast<Transform>().ToArray();
        Initialize();
    }

    public void SetOwner(Ship ship)
    {
        this.ship = ship;
    }

    public virtual void Update()
    {
        if (!IsShooting)
        {
            timer = newShot - Time.time;

            if (timer < 1.5f)
            {
                AboutToShoot?.Invoke(true);
            }
        }
        
        if (Time.time > newShot && AutoAttack)
        {
            AboutToShoot?.Invoke(false);
            newShot = Time.time + FireRate;

            Shoot();
        }
    }

    public virtual void Initialize() { }

    public abstract void Shoot();

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
