using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultEnemyProjectile : BaseProjectile
{
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

    public override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var destroyable = other.GetComponent<IDamagable>();
            destroyable.TakeDamage(Damage);
            DestoryNow();
        }
    }
}
