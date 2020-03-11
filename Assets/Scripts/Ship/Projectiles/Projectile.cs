using TheGamerUrso.PoolSystem;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    protected Rigidbody rigid;
 
    [SerializeField]protected float damage;
    [SerializeField]protected float speed;
    [SerializeField]protected PoolGameObjectType ExplosionPrefab;

    protected GameObject explosion;
    protected Transform EffectsHolder;
    protected TrailRenderer trailRenderer;

    private void OnEnable()
    {
        
    }

    protected virtual void OnAwake()
    {

    }

    private void Awake()
    {
        OnAwake();
        SetInitialReference();     
    }
    private void Start()
    {
        OnStart();
    }

    public virtual void SetInitialReference() {
        rigid = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
    }

    protected virtual void OnStart()
    {

    }
    private void FixedUpdate()
    {
        Movement();
    }

    public abstract void Movement();

    public virtual void DestoryNow(){}

    public void setDamage(float newDamage)
    {
        damage = newDamage;
    }

    public float getDamage()
    {
        return damage;
    }
}