using DG.Tweening;
using System.Collections;
using UnityEngine;

public class SimpleAI : BaseEnemyAI
{

    public override void Initialize()
    {
        base.Initialize();

        m_XVel = enemy.GetShipStatsSystem().GetSpeed() / 2;
        m_ZVel = enemy.GetShipStatsSystem().GetSpeed();
        Direction = 0;
    }

    public override void Move()
    {
        //With transform
        movement = (transform.forward * m_ZVel) + (transform.right * m_XVel);
        movement.x *= Direction;
        //transform.localPosition += movement * Time.deltaTime;


        //With Translate
        //  transform.Translate(movement * Time.deltaTime,Space.Self);

        //With AddForce
        // rigid.AddForce(movement * 200 * Time.deltaTime);

        //Move Position
         rigid.MovePosition(transform.position + (movement * Time.deltaTime));

        //with Velocity

        // rigid.velocity = (-transform.forward * m_ZVel) + (-transform.right * (Direction * m_XVel)) * Time.deltaTime;

        //transform.position = Vector3Extention.GetClampVector3(transform);

        CheckOutOfSight();

    }



}