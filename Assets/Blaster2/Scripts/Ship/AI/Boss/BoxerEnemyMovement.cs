using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoxerEnemyMovement : BaseBossEnemyMovement
{
    [Header("Movement")]
    public bool attacking;
    public GameObject ShipPivot;
    private float cooldown;
    public override void Movement()
    {
        if (enemy.CurrentHealth > 0)
        {
            if (!attacking && cooldown > 0)
            {
                cooldown -= Time.deltaTime;
            }

            if (cooldown <= 0)
            {
                if (bossDestroyableParts.Length > 0)
                {
                    List<BossDestroyablePart> newList = bossDestroyableParts.Where(x => x.CurrentHealth > 0).ToList();
                    int rand = UnityEngine.Random.Range(0, newList.Count);
                    if (newList.Count > 0)
                    {
                        cooldown = UnityEngine.Random.Range(4, 8);
                        newList[rand].Attack((x) =>
                        {
                            attacking = x;
                        });
                    }
                }
            }

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
}
