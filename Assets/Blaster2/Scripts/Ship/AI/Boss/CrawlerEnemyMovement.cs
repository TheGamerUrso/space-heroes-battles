using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlerEnemyMovement : BaseBossEnemyMovement
{
    [Header("Movement")]
    public bool CurclularMove;
    public float angle = 0;
    public float radius = 5;

    public override void OnEnable()
    {
        int changeNum = Random.Range(0, 100);
        if (changeNum >= 50)
        {
            speed *= -1;
        }
    }

    public override void Start()
    {
        base.Start();
        speed = (2 * Mathf.PI) / 2; //2*PI in degress is 360, so you get 5 seconds to complete a circle
    }

    public override void Movement()
    {
        if(CurclularMove)
            CircularMovement();
    }
    public override void EnableMovement()
    {
        CurclularMove = true;
    }

    public void CircularMovement()
    {
        angle += speed * Time.deltaTime; //if you want to switch direction, use -= instead of +=

        var xVel = Mathf.Cos(angle) * radius + .8f;
        var zVel = Mathf.Sin(angle) * radius + 80;

        Vector3 newPos = targetPosition;
        newPos.x = xVel;
        newPos.z = zVel;
        targetPosition = newPos;
    }

    public override void OnBossPhaseChangedHandled(int Phase)
    {

    }
}
