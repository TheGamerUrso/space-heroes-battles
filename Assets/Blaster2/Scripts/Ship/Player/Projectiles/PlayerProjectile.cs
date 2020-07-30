using UnityEngine;

public class PlayerProjectile : Projectile
{
    public override void Setup(WeaponScript weaponScript)
    {
        this.weaponScript = weaponScript;
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
        var explode = PoolManager.Instance.GetObjectFromPool(PoolGameObjectType.BulletExplosion);
        explode.SetActive(true);
        explode.transform.position = transform.position;
        gameObject.SetActive(false);
    }

    public void OnTriggerEnter(Collider other)
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