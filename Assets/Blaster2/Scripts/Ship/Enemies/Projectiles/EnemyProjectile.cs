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

    public override void Setup(WeaponScript weaponScript)
    {
        this.weaponScript = weaponScript;
    }

    public override void Movement()
    {
        Vector3 newPos = transform.position;
        newPos += shootDir * speed * Time.deltaTime;
        transform.position = newPos;

 
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
        if (other.CompareTag("Player"))
        {
            var destroyable = other.GetComponent<IDamagable>();
            destroyable.TakeDamage(Damage);
            DestoryNow();
        }
    }
}