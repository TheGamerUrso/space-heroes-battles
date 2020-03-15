using TheGamerUrso;
using TheGamerUrso.PoolSystem;
using TheGamerUrso.Utils;
using UnityEngine;

public class EnemyProjectile : Projectile
{
    public GameObject Target;
    public Vector3 TargetLastPosition;

    public bool FollowTarget;

    private void OnDisable()
    {
        if (trailRenderer)
            trailRenderer.Clear();
    }

    public override void Setup(Vector3 shootDir, float dmg)
    {
        this.shootDir = shootDir;
       // transform.eulerAngles = new Vector3(0, Utilities.GetAngleFromVectorFloat3D(shootDir), 0);
        if (dmg > 0)
            Damage = dmg;
    }
    public override void Movement()
    {
        transform.position += shootDir * speed * Time.deltaTime;

 
        if (transform.position.z < Constants.m_ZMin)
        {
            gameObject.SetActive(false);
        }
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