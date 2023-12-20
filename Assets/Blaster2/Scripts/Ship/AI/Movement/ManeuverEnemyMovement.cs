using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManeuverEnemyMovement : BaseEnemyMovement
{    
    [Header("Movement")]
    protected int Direction;
    protected bool directionChanged;
    float maneuverTimer = 2;
    
    public override void Movement()
    {
        maneuverTimer -= Time.deltaTime;

        if (maneuverTimer <= 0)
        {
            maneuverTimer = UnityEngine.Random.Range(2, 4);
            Maneuver();
        }


        movement = (transform.forward * Speed) + (transform.right * (Speed / 2));
        movement.x *= Direction;
        transform.position += movement * Time.deltaTime;


        if (transform.position.x > Constants.m_XMax)
        {
            Direction = 1;
        }
        else if (transform.position.x < Constants.m_XMin)
        {
            Direction = -1;
        }

        CheckOutOfSight();
    }

    private void Maneuver()
    {
        int random = UnityEngine.Random.Range(0, 100);

        if (random <= 33.33)
        {
            Direction = 1;
        }
        else if (random > 33.33 && random <= 66.66)
        {
            Direction = -1;
        }
        else
        {
            Direction = 0;
        }

        Direction = 0;
    }
}
