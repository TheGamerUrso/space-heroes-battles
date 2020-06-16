using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileBall : EnemyProjectile
{
    protected override void OnEnable()
    {
        base.OnEnable();
    }

    public override void Movement()
    {
        Vector3 newPos = transform.position;
        newPos += transform.forward * speed * Time.deltaTime;
        transform.position = newPos;


        if (transform.position.z < Constants.m_ZMin)
        {
            DestoryNow();
        }
    }
}
