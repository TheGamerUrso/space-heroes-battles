using System.Collections;
using System.Collections.Generic;
using TheGamerUrso;
using TheGamerUrso.Utils;
using UnityEngine;

public class FollowAI : SimpleAI {
    public GameObject m_Target;
    private Vector3 ShootDirection;
    public float speed;
    public override void Initialize()
    {
        m_Target = GameObject.FindGameObjectWithTag("Player");
        RotationExtentionUtilities.UpdateShipRotation(transform,new Vector3(-m_XVel, 0, m_ZVel));
    }
   
    private void LateUpdate()
    {
        if (m_Target)
        {
            lookAt(transform, ShootDirection);
        }

        if (Utilities.GameObjectIsOutOfCameraVision(transform, -150))
        {
            Initialize();
        }
    }

   
    public void lookAt(Transform transform, Vector3 diff)
    {
        diff.Normalize();
        float rot_y = Mathf.Atan2(diff.x, diff.z) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0f, rot_y, 0f);
    }

   
    public override void Move()
    {
        Vector3 away = transform.position - m_Target.transform.position;
        float direction = away.magnitude;

        if (Vector3.Distance(transform.position, m_Target.transform.position) >= 1)
        {
            transform.position -= transform.forward * speed * Time.deltaTime;
        }

        transform.position = Vector3.MoveTowards(transform.position, m_Target.transform.position, 15 * Time.deltaTime);
     
    }

    //====================================================================================================
    public void SetTarget(GameObject target)
    {
        m_Target = target;
    }
}
