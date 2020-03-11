using TheGamerUrso;
using TheGamerUrso.PoolSystem;
using UnityEngine;

public class PlayerProjectile : Projectile
{
    private GameObject explosion;
    private IDestroyable Target;


    private void OnDisable()
    {
        if (trailRenderer)
            trailRenderer.Clear();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    public override void Movement()
    {
        rigid.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);

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
        if (other.tag.Equals(Constants.ENEMYTAG))
        {
            IDestroyable destroyable = other.GetComponent<IDestroyable>();
            if (destroyable != null)
            {
                destroyable.TakeDamage(damage);
            }
            DestoryNow();
        }
    }

    public GameObject FindClosestEnemy()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            if (go.GetComponent<BaseEnemy>())
            {
                Vector3 diff = go.transform.position - position;
                float curDistance = diff.sqrMagnitude;
                if (curDistance < distance)
                {
                    closest = go;
                    distance = curDistance;
                }
            }
        }
        return closest;
    }
}