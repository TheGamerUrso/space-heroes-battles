using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    protected Rigidbody rb;

    [SerializeField]
    protected float Damage;

    [SerializeField]
    protected PoolGameObjectType ExplosionPrefab;

    [SerializeField]
    protected float speed;

    protected Transform EffectsHolder;

    [SerializeField] protected bool FollowTarget = false;

    private void Awake()
    {
        SetInitialReference();
    }

    public virtual void SetInitialReference() { }

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