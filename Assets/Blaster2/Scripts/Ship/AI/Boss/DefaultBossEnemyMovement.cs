using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultBossEnemyMovement : BaseBossEnemyMovement
{
    public override void Movement()
    {
        transform.position = Vector3.Lerp(transform.position, Positions[currentPos], speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, Positions[currentPos]) < 1)
        {
            changePositionTimer = UnityEngine.Random.Range(2, 4);
            currentPos++;
            if (currentPos > 3)
            {
                currentPos = 0;
            }
        }
    }
}
