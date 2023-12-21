using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseProjectile : MonoBehaviour
{
    protected BaseWeapon baseWeapon;
    protected Rigidbody rigid;
    protected Vector3 shootDir;

    public float Damage
    {
        get { return baseWeapon.Damage; }
    }

    [SerializeField] protected float speed;
    [SerializeField] protected PoolGameObjectType ExplosionPrefab;

    protected MeshRenderer meshRenderer;
    protected GameObject explosion;
    protected Transform EffectsHolder;
    protected TrailRenderer trailRenderer;

    protected virtual void OnEnable() { }
   protected virtual void OnDisable()
    {
        if (trailRenderer)
            trailRenderer.Clear();
    }
    
    protected virtual void OnAwake()
    {

    }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        OnAwake();
    }

    private void Start() => OnStart();

    public virtual void SetOwner(BaseWeapon baseWeapon)
    {
        this.baseWeapon = baseWeapon;
    }
    public virtual void OnStart() { }
    private void LateUpdate() => Movement();

    public abstract void Movement();

    public virtual void DestoryNow()
    {
        var explode = PoolManager.Instance.GetObjectFromPool(PoolGameObjectType.BulletExplosion);
        explode.SetActive(true);
        explode.transform.position = transform.position;
        gameObject.SetActive(false);
    }

    public virtual void SetShootDir(Vector3 shootDir)
    {
        this.shootDir = shootDir;
    }

     public abstract void OnTriggerEnter(Collider other);
}
