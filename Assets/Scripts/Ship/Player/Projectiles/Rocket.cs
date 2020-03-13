using TheGamerUrso;
using TheGamerUrso.Utils;
using UnityEngine;

public class Rocket : PlayerProjectile
{
    public bool HomeMissleType = false;
    public GameObject m_Target;

    protected override void OnEnable()
    {
        if (HomeMissleType)
        {
            m_Target = Utilities.GetClosest(transform.position, 10);
            shootDir = transform.forward;
            if (m_Target != null)
            {
                shootDir = (m_Target.transform.position - transform.position).normalized;
            }
        }
    }

    public override void OnStart()
    {
        base.OnStart();
    }

    public override void Movement()
    {
        if (HomeMissleType && m_Target)
        {
            Vector3 dest = m_Target.transform.position;
            Vector3 rockPos = transform.position;
            Vector3 def = dest - rockPos;

            transform.position += shootDir * speed * Time.deltaTime;

            transform.eulerAngles = new Vector3(0, Utilities.GetAngleFromVectorIn3D(def), 0);
        }
        else
        {
            base.Movement();
            transform.eulerAngles = new Vector3(0, 0, 0);
        }

        if (transform.position.z > Constants.m_ZMax)
        {
            gameObject.SetActive(false);
        }
    }


}