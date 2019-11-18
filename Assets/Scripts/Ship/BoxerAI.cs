using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxerAI : BaseBossEnemyAI
{

    public override IEnumerator MoveVerticalWithDelay(int hitIndex)
    {
        int waitTime = Random.Range(2, 4);
        int waitTillComeBack = Random.Range(2, 4);

        while (pathMagnitude > 1)
        {
            yield return new WaitForSeconds(waitTime);
        }

        if (currentPointToFollowIndex == 0)
        {
            currentPointToFollowIndex = 1;
        }
        else if (currentPointToFollowIndex == 1)
        {
            currentPointToFollowIndex = 0;
        }

        yield return new WaitForSeconds(waitTillComeBack);
        currentPointToFollowIndex = 0;

        yield return new WaitForSeconds(1);
        m_IsMovingVertical = false;
        cooldown = Random.Range(4, 6);
        hitIndex = 0;
    }
}
