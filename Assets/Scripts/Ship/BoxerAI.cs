using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoxerAI : BaseBossEnemyAI
{
    public Punch[] punches;
    public bool attacking;

    public override void Enter()
    {
        base.Enter();
    }

    public override void Setup()
    {
        base.Setup();
        cooldown = UnityEngine.Random.Range(4, 8);
    }

    public override void Move()
    {
        if (baseBoss.CurrentHealth > 0)
        {
            if (!attacking)
            {
                cooldown -= Time.deltaTime;
            }

            if (cooldown <= 0)
            {
                cooldown = UnityEngine.Random.Range(4, 8);
                int rand = UnityEngine.Random.Range(0, punches.Length);
                List<Punch> newList = punches.Where(x => x.currentHealth > 0).ToList();
                newList[rand].Attack((x) => { attacking = x; Debug.Log("Punch" + attacking); });
            }
        }
    }

}
