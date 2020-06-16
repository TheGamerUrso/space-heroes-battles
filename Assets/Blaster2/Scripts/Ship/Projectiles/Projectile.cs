using TheGamerUrso.PoolSystem;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    protected Rigidbody rigid;
    protected Vector3 shootDir;
    [SerializeField] protected float damage;
    public float Damage
    {
        get { return damage; }
        set { damage = value; }
    }

    [SerializeField] protected float speed;
    [SerializeField] protected PoolGameObjectType ExplosionPrefab;

    protected GameObject explosion;
    protected Transform EffectsHolder;
    protected TrailRenderer trailRenderer;

    protected virtual void OnEnable() { } 

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
    }
    public virtual void Setup(Vector3 shootDir,float dmg = 1) { }
    public virtual void OnStart() { }
    private void LateUpdate() => Movement();

    public abstract void Movement();

    public virtual void DestoryNow() { }
}