using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultEnemyMovement : BaseEnemyMovement
{
    public override void Move()
    {
        movement = (transform.forward * Speed) + (transform.right * (Speed / 2));
        transform.position += movement * Time.deltaTime;
        CheckOutOfSight();
    }
}
