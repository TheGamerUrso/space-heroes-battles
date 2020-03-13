using TheGamerUrso;
using TheGamerUrso.Utils;
using UnityEngine;

public class FastAI : SimpleAI
{
    private int num;

    public float m_Rotation;
    public Vector2 m_Velocity;

    public override void Initialize()
    {
        num = Random.Range(0, 2);
        SetFastEnemyDirection(num);
        m_ZVel = m_Velocity.y;
    }

    public override void Move()
    {
        if(rigid == null)
        {
            rigid = GetComponent<Rigidbody>();
        }

        rigid.velocity = (-transform.forward * m_ZVel) + (transform.right * m_XVel);

        if (Utilities.GameObjectIsOutOfCameraVisionOnXandYAxis(transform, num))
        {
            if (Loop)
            {
                GetComponent<Animator>().SetTrigger("Enter");
                num = Random.Range(0, 2);
                SetFastEnemyDirection(num);
            }
            else
            {
                Destroy(gameObject);
            }
        }

    }

    //====================================================================================================
    public void SetFastEnemyDirection(int num)
    {
        if (num == 0)
        {
            m_XVel = -m_Velocity.x;
            transform.localPosition = new Vector3(Constants.m_XMax, 0, Constants.m_ZMax);
            transform.localEulerAngles = new Vector3(0, m_Rotation, 0);
        }
        else if (num == 1)
        {
            m_XVel = m_Velocity.x;
            transform.localPosition = new Vector3(Constants.m_XMin, 0, Constants.m_ZMax);
            transform.localEulerAngles = new Vector3(0, -m_Rotation, 0);
        }
    }
}
