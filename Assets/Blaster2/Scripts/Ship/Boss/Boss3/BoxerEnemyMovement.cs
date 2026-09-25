using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoxerEnemyMovement : BaseBossEnemyMovement
{
    [Header("Movement")]
    public GameObject ShipPivot;

    public override void Move()
    {
        if(!enemy.GetComponent<BossEnemy>().StartBattle)return;
        transform.position = Vector3.Lerp(transform.position, Positions[currentPos], speed * Time.deltaTime);
    }
}
