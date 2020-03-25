using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlerAI : BaseBossEnemyAI
{
    #region Curcular Movement Settings
    [Header("Curcular Movement Settings")]
    public bool CurclularMove;
    public float angle = 0;
    float speed = (2 * Mathf.PI) / 5; //2*PI in degress is 360, so you get 5 seconds to complete a circle
    public float radius = 5;
    #endregion

    public override void Enter()
    {
        base.Enter();
        int changeNum = Random.Range(0, 100);
        if (changeNum >= 50)
        {
            speed *= -1;
        }
    }

    public override void EnableMovement()
    {
        CurclularMove = true;
    }

    public override void Move()
    {
        base.Move();
        if (CurclularMove)
        {
            CircularMovement();
        }
    }

    public void CircularMovement()
    {
        angle += speed * Time.deltaTime; //if you want to switch direction, use -= instead of +=

        xVel = Mathf.Cos(angle) * radius + .8f;
        zVel = Mathf.Sin(angle) * radius + 100;

        Vector3 newPos = transform.position;
        newPos.x = xVel;
        newPos.z = zVel;
        transform.position = newPos;
    }
   

}
