using TheGamerUrso;
using TheGamerUrso.PoolSystem;
using UnityEngine;

public class EnemyProjectile : Projectile
{
    public GameObject Target;
    public Vector3 TargetLastPosition;

    public bool FollowTarget;

    protected override void OnAwake()
    {
        base.OnAwake(); 
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        rigid.velocity = Vector3.zero;
    }

    protected override void OnStart()
    {
        base.OnStart();

    }
    public override void Movement()
    {
        //rigid.MovePosition(transform.position  + (transform.forward * speed * Time.deltaTime));   

        //if (transform.position.z < Constants.m_ZMin)
        //{
        //    gameObject.SetActive(false);
        //}
    }

    public override void DestoryNow()
    {
        explosion = PoolManager.Instance.GetObjectFromPool(PoolGameObjectType.BulletExplosion);
        explosion.transform.SetPositionAndRotation(transform.position + Vector3.up * 2, Quaternion.identity);
        gameObject.SetActive(false);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(Constants.PLAYTERTAG))
        {
            IDestroyable destroyable = other.GetComponent<IDestroyable>();
            destroyable.TakeDamage(damage);
            DestoryNow();
        }
    }
}