using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxerAI : BossAI
{
    public float cooldown;

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Move()
    {
        base.Move();

        if (cooldown > 0)
        {
            cooldown -= Time.deltaTime;
        }

    }
    public override void ChangeWaypoint(int hitIndex)
    {
        if (cooldown <= 0)
        {
            if (!m_IsMovingVertical)
            {
                m_IsMovingVertical = true;
                StartCoroutine(PushForward(hitIndex));
            }
        }
    }


    private IEnumerator PushForward(int hitIndex)
    {    
        currentPointToFollowIndex = 1;
        yield return new WaitForSeconds(1);
        currentPointToFollowIndex = 0;
        m_IsMovingVertical = false;
        hitIndex = 0;
        cooldown = UnityEngine.Random.Range(4, 6); 
    }
}
