using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    protected WeaponScript weaponScript;
    protected Rigidbody rigid;
    protected Vector3 shootDir;

    public float Damage
    {
        get { return weaponScript.Damage; }
    }

    [SerializeField] protected float speed;
    [SerializeField] protected PoolGameObjectType ExplosionPrefab;

    protected MeshRenderer meshRenderer;
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
        meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    public virtual void Setup(WeaponScript weaponScript) { }
    public virtual void OnStart() { }
    private void LateUpdate() => Movement();

    public abstract void Movement();

    public virtual void DestoryNow() { }

    public virtual void SetShootDir(Vector3 shootDir)
    {
        this.shootDir = shootDir;
    }
}