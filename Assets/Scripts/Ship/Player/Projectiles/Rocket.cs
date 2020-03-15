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
            m_Target = Utilities.GetClosest(Constants.ENEMYTAG,transform.position, Mathf.Infinity);
            if (m_Target != null)
            {
                Debug.Log("Attacking " + m_Target, gameObject);
            }else if(m_Target == null)
            {
                shootDir = transform.forward;
            }
        }
    }

    public override void OnStart()
    {
        base.OnStart();
    }

    public override void Movement()
    {
        if (HomeMissleType)
        {
            if (m_Target != null)
            {
                Debug.DrawLine(transform.position, m_Target.transform.position, Color.red);

                shootDir = (m_Target.transform.position - transform.position).normalized;

                if (!m_Target.activeInHierarchy)
                {
                    m_Target = null;
                }

            }
            else if (m_Target == null)
            {
                shootDir = transform.forward;
            }

        
            transform.position += shootDir * speed * Time.deltaTime;

            transform.rotation = Quaternion.LookRotation(shootDir);
        }


        if (transform.position.z > Constants.m_ZMax)
        {
            gameObject.SetActive(false);
        }
    }

}