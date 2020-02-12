using TheGamerUrso.PoolSystem;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    protected Rigidbody rb;

    [SerializeField]
    protected float Damage;
    protected Rigidbody rigid;
    [SerializeField]
    protected PoolGameObjectType ExplosionPrefab;

    [SerializeField]
    protected float speed;

    protected Transform EffectsHolder;
    protected TrailRenderer trailRenderer;
    [SerializeField] protected bool FollowTarget = false;

    private void Awake()
    {
        SetInitialReference();

        trailRenderer = GetComponent<TrailRenderer>();
    }

    public virtual void SetInitialReference() {
        rigid = GetComponent<Rigidbody>();
    }

    public void setDamage(float newDamage)
    {
        Damage = newDamage;
    }

    public float getDamage()
    {
        return Damage;
    }

    public abstract void Movement();

    public void BulletRotation(GameObject target, Vector3 rot)
    {
        target.transform.eulerAngles = rot;
    }

    public virtual void DestoryNow(){}
}