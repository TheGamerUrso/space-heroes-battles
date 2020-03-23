using DG.Tweening;
using UnityEngine;

public class ManeuverEnemy : SimpleAI
{
    private bool firstTime;

    public override void Setup()
    {
        base.Setup();
        int waitTime = UnityEngine.Random.Range(2, 4);
        InvokeRepeating("Maneuver", 1, waitTime);

    }

    public override void Move()
    {
        movement = (transform.forward * m_ZVel) + (transform.right * m_XVel);
        movement.x *= Direction;
        transform.position += movement * Time.deltaTime;
        CheckOutOfSight();
    }

    private void LateUpdate()
    {
        if (transform.position.x > Constants.m_XMax)
        {
            Direction = 1;
        }
        else if (transform.position.x < Constants.m_XMin)
        {
            Direction = -1;
        }
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
