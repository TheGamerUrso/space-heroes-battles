using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public abstract class WeaponScript : MonoBehaviour
{
    public Action<bool> AboutToShoot;

    protected Ship ship;
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

    public bool AutoAttack { get; set; }


    public AudioClip SoundSFX { get { return weaponData.ShootSFX; } }
    public PoolGameObjectType ProjectilePrefab { get { return weaponData.m_Projectile; } }

    public virtual void Awake() { }
    public virtual void Start()
    {
        source = GetComponent<AudioSource>();
        Cannons = transform.Cast<Transform>().ToArray();
        ship = GetComponentInParent<Ship>();
        Initialize();
    }

    public virtual void Update() { Shoot(); }

    public virtual void Initialize() { }

    public virtual void Shoot() { }

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

