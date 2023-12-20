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
    protected virtual void OnAwake() => SetInitialReference();


    private void Awake()
    {
        OnAwake();
    }

    private void Start() => OnStart();

    public virtual void SetInitialReference()
    {
        rigid = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    public virtual void Setup(BaseWeapon baseWeapon) { }
    public virtual void OnStart() { }
    private void LateUpdate() => Movement();

    public abstract void Movement();

     public virtual void DestoryNow()
    {
        explosion = PoolManager.Instance.GetObjectFromPool(PoolGameObjectType.BulletExplosion);
        explosion.transform.SetPositionAndRotation(transform.position + Vector3.up * 2, Quaternion.identity);
        explosion.SetActive(true);
        gameObject.SetActive(false);
    }

    public virtual void SetShootDir(Vector3 shootDir)
    {
        this.shootDir = shootDir;
    }

     public abstract void OnTriggerEnter(Collider other);
}
