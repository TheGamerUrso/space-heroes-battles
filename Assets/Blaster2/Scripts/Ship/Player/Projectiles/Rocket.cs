using UnityEngine;

public class Rocket : PlayerProjectile
{
    public GameObject m_Target;

    protected override void OnEnable()
    {
        m_Target = HelperUtils.GetClosest(Constants.ENEMYTAG, transform.position, Mathf.Infinity);
        if (m_Target != null)
        {
   
        }
        else if (m_Target == null)
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
                m_Target = HelperUtils.GetClosest(Constants.ENEMYTAG, transform.position, Mathf.Infinity);
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

}