using TheGamerUrso.PoolSystem;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    protected Rigidbody rigid;

    [SerializeField]protected float damage;
    [SerializeField]protected float speed;
    [SerializeField]protected PoolGameObjectType ExplosionPrefab;

    protected Transform EffectsHolder;
    protected TrailRenderer trailRenderer;

    private void Awake()
    {
        SetInitialReference();     
    }

    public virtual void SetInitialReference() {
        rigid = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
    }

    public void setDamage(float newDamage)
    {
        damage = newDamage;
    }

    public float getDamage()
    {
        return damage;
    }
    private void FixedUpdate()
    {
        Movement();
    }

    public abstract void Movement();

    public virtual void DestoryNow(){}
}