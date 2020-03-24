using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxerBossBattle : BaseBossBattle
{

    public override void BossHit(int hitIndex, int numberOfHits)
    {
        base.BossHit(hitIndex, numberOfHits);

        if (Random.Range(0, 100) >= 50)
        {
            if (HealthPresentage % 40 == 0)
            {
                GetComponent<BossAI>().ChangeWaypoint(hitIndex);
            }
        }
    }
}
