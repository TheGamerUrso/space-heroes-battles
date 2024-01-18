using UnityEngine;

public class RocketProjectile : BaseProjectile
{
    public GameObject m_Target;
    public LayerMask enemyLayer;

    protected override void OnEnable()
    {
        var acceptedTarget = HelperUtils.GetClosest(Constants.ENEMYTAG, transform.position, Mathf.Infinity, enemyLayer);
        if (acceptedTarget!=null && !acceptedTarget.GetComponent<AsteroidCollider>())
        {
            m_Target = acceptedTarget;
        }
        if (m_Target == null)
        {
            shootDir = Vector3.forward;
        }
    }

    public override void OnStart()
    {
        base.OnStart();
    }

    public override void Movement()
    {
        if (m_Target != null)
        {
            shootDir = (m_Target.transform.position - transform.position).normalized;

            if (!m_Target.activeInHierarchy)
            {
                var acceptedTarget = HelperUtils.GetClosest(Constants.ENEMYTAG, transform.position, Mathf.Infinity, enemyLayer);
                if (acceptedTarget != null && !acceptedTarget.GetComponent<AsteroidCollider>())
                {
                    m_Target = acceptedTarget;
                }
                else
                {
                    m_Target = null;
                    shootDir = Vector3.forward;
                }
            }
        }

        if (m_Target == null)
        {
            shootDir = Vector3.forward;
        }

        transform.Translate(shootDir * speed * Time.deltaTime, Space.World);

        transform.rotation = Quaternion.LookRotation(shootDir, transform.up);



        if (transform.position.z > Constants.m_ZMax)
        {
            gameObject.SetActive(false);
        }
    }

    public override void DestoryNow()
    {
        var explode = PoolManager.Instance.GetObjectFromPool(PoolGameObjectType.BulletExplosion);
        explode.SetActive(true);
        explode.transform.position = transform.position;
        gameObject.SetActive(false);
    }
    public override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            var destroyable = other.GetComponent<IDamagable>();
            if (destroyable != null)
            {
                destroyable.TakeDamage(Damage);
            }
            DestoryNow();
        }
    }
}