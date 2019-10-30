using System.Collections;
using System.Collections.Generic;
using TheGamerUrso;
using UnityEngine;
public enum DirectionType
{
    left,right
}
public class SideAI : SimpleAI
{

    [Header("SideAI")]
    public DirectionType m_Enemy3Direction = DirectionType.left;
    private Vector3 m_LeftStartingPoint;
    private Vector3 m_RightStartingPoint;
    public Vector2 m_XVelSpeedRange;
    public Vector2 m_YVelSpeedRange;
    private Vector3 m_StartingPosition;

    public override void Initialize()
    {
        m_LeftStartingPoint = new Vector3(Constants.m_XMin, 0, Constants.m_ZMax);
        m_RightStartingPoint = new Vector3(Constants.m_XMax, 0, Constants.m_ZMax);

        SetPosition();
    }

    //====================================================================================================
    public override void Move()
    {
        m_ZVel += 2.5f * Time.deltaTime;

        transform.Translate(transform.forward * m_ZVel * Time.deltaTime);
       
        m_ZVel = Mathf.Clamp(m_ZVel, 0, 25);

        if (Utilities.CheckOutOfCamera(transform, Loop))
        {
            GetComponent<Animator>().SetTrigger("Enter");
            SetPosition();
        }
    }

    //====================================================================================================
    public void SetPosition()
    {
        int index = Random.Range(0, 100);
        if (index <= 50)
        {
            m_Enemy3Direction = DirectionType.left;
        }
        else if (index > 50)
        {
            m_Enemy3Direction = DirectionType.right;
        }

        switch (m_Enemy3Direction)
        {
            case DirectionType.left:
                transform.localPosition = m_LeftStartingPoint;
                m_StartingPosition = transform.position;
                RotationExtentionUtilities.RotateObject(transform,-90f);
                break;
            case DirectionType.right:
                transform.localPosition = m_RightStartingPoint;
                m_StartingPosition = transform.position;
                RotationExtentionUtilities.RotateObject(transform, 90f);
                break;
            default:
                break;
        }


    }
}


