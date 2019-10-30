using TheGamerUrso;
using UnityEngine;

public class Rocket : PlayerProjectile
{
    public bool HomeMissleType = false;
    public GameObject m_Target;

    private void OnEnable()
    {
        if (HomeMissleType)
        {
            m_Target = FindClosestEnemy();
        }
    }

    public override void Movement()
    {
        if (HomeMissleType && m_Target)
        {
            Vector3 dest = m_Target.transform.position;
            Vector3 rockPos = transform.position;
            Vector3 def = dest - rockPos;

            transform.position = Vector3.MoveTowards(transform.position
                , m_Target.transform.position,
                speed * Time.deltaTime);

            lookAt(transform, def);
        }
        else
        {
            base.Movement();
            transform.eulerAngles = new Vector3(0, 0, 0);
           // if (m_Target == null)
             //   m_Target = FindClosestEnemy();
        }

        if (m_Target != null && m_Target.activeInHierarchy == false)
        {
            m_Target = null;
         
        }

        if(transform.position.z > Constants.m_ZMax)
        {
            gameObject.SetActive(false);
        }
    }

    public void lookAt(Transform transform, Vector3 diff)
    {
        diff.Normalize();
        float rot_y = Mathf.Atan2(diff.x, diff.z) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0f, rot_y, 0f);
    }
}