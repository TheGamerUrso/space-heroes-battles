using UnityEngine;

public class PlayerProjectile : Projectile
{
    public override void Setup(Vector3 shootDir, float dmg)
    {
        this.shootDir = shootDir;
        //transform.eulerAngles = new Vector3(0, Utilities.Get(shootDir), 0);
        if (dmg > 0)
            Damage = dmg;

    }

    private void OnDisable()
    {
        if (trailRenderer)
            trailRenderer.Clear();
    }

    public override void Movement()
    {
        transform.position += shootDir * speed * Time.deltaTime;

        if (transform.position.z > Constants.m_ZMax)
        {
            gameObject.SetActive(false);
        }
    }

    public override void DestoryNow()
    {
        GameObject explode = PoolManager.Instance.GetObjectFromPool(PoolGameObjectType.BulletExplosion);
        explode.transform.position = transform.position;
        //explosion.transform.position = transform.position + Vector3.up * 2;
        //explosion.transform.rotation = Quaternion.identity;

        gameObject.SetActive(false);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            var destroyable = other.GetComponent<IDamagable>();
            if (destroyable != null)
            {
                destroyable.TakeDamage(damage);
            }
            DestoryNow();
        }
    }
}