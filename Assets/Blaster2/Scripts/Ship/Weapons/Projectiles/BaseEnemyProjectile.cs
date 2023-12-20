using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyProjectile : BaseProjectile
{
    public GameObject Target;
    public Vector3 TargetLastPosition;

    public bool FollowTarget;

    private void OnDisable()
    {
        if (trailRenderer)
            trailRenderer.Clear();
    }

    public override void Setup(BaseWeapon baseWeapon)
    {
        this.baseWeapon = baseWeapon;
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
