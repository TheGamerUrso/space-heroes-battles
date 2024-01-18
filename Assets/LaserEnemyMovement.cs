using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEnemyMovement : BaseEnemyMovement
{
    public int direction;
    public bool left;
    public override void Start()
    {
        base.Start();
        var randXPos = 20;
        var yRot = 90;
        var randNum = UnityEngine.Random.Range(1,101);
        if (randNum <= 50)
        {
            randXPos = -20;
            left = true;
            yRot = 90;
        }
        else
        {
            randXPos = 20;
            left = false;
            yRot = -90;
        }
        transform.position = new Vector3(randXPos,0,140);
        transform.SetPositionAndRotation(new Vector3(randXPos,0,140),Quaternion.Euler(0,yRot,0));
    }
    public override void Movement()
    {
        direction = left ? 1 : -1;
        movement = transform.right * (direction) * (Speed / 2);
        transform.position += movement * Time.deltaTime;
        CheckOutOfSight();
    }
}
