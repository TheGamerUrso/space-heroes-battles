using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultBossEnemyMovement : BaseBossEnemyMovement
{
    public float speedByPhase;
    public override void Update()
    {
        base.Update();
        if(!shipOwner.GetComponent<BossEnemy>().StartBattle)return;
        changePositionTimer-=Time.deltaTime;
        if(changePositionTimer<0)
        {
            changePositionTimer = 0;
        }

        if(changePositionTimer>0)return;

        float dist = Vector3.Distance(transform.position, Positions[currentPos]);
        Debug.Log(dist);
        if (Vector3.Distance(transform.position, Positions[currentPos]) < 11)
        {
            changePositionTimer = 2;    

            if(RandomMovement)
            {
                currentPos = Random.Range(0,Positions.Length);
            }else
            {
                  currentPos++;
            }
          
            if (currentPos > Positions.Length-1)
            {
                currentPos = 0;
            }
            targetPosition = Positions[currentPos];
        }
    }
    public override void Movement()
    {
        transform.position = Vector3.Lerp(transform.position, Positions[currentPos], speed * Time.deltaTime);      
    }

    public override void OnBossPhaseChangedHandled(int Phase)
    {
       if(Phase == 2)
       {
        speed += .1f;
       }
    }
}
