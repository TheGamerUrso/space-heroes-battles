using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy3AI : SimpleAI
{
    private GameObject target;
    public float changeWaypointTimer;
    public Transform[] Waypoints;
    public int currentWaypoint;

    public override void Move()
    {
        if (transform.position.z > 50)
        {
            transform.position -= transform.forward * zVel * Time.deltaTime;
        }
        else
        {
            target = GameObject.FindObjectOfType<PlayerShip>().gameObject;

            Vector3 diffrence = target.transform.position - transform.position;
            lookAt(transform, -diffrence);
        }

        if (transform.position.z <= 50)
        {
            if (changeWaypointTimer >= 0)
            {
                changeWaypointTimer -= Time.deltaTime;
            }
            else
            {
                changeWaypointTimer = Random.Range(2, 4);
                if (currentWaypoint < Waypoints.Length - 1)
                {
                    currentWaypoint++;
                }
                else
                {
                    currentWaypoint = 0;
                }
            }
        }
    }


    private void FixedUpdate()
    {
        if (transform.position.z <= 50)
        {
            Vector3 newPos = new Vector3(Waypoints[currentWaypoint].position.x, transform.position.y, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, newPos, .5f);
        }
    }
    //====================================================================================================
    public void lookAt(Transform transform, Vector3 diff)
    {
        diff.Normalize();
        float rot_y = Mathf.Atan2(diff.x, diff.z) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0f, rot_y, 0f);
    }
}
