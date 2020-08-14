using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
public class BoxerMove : EnemyMove
{
    public BaseBossEnemy BossEnemy;
    public Vector3[] Positions;
    public int currentPos;
    public bool StartBattle;
    public float changePositionTimer;

    public Punch[] punches;
    public bool attacking;
    public GameObject ShipPivot;
    private float cooldown;

    public override void Start()
    {
        StartCoroutine(DelayStart());
    }

    public override void LateUpdate()
    {
        if (StartBattle)
        {
            if (enemy.CurrentHealth > 0)
            {
                if (!attacking && cooldown > 0)
                {
                    cooldown -= Time.deltaTime;
                }

                if (cooldown <= 0)
                {
                    if (punches.Length > 0)
                    {
                        List<Punch> newList = punches.Where(x => x.CurrentHealth > 0).ToList();
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

    IEnumerator DelayStart()
    {
        yield return new WaitForSeconds(4);

        BossEnemy.EnableAllWeapon();
        BossEnemy.EnableColliders(true);
        BossEnemy.HealthBar.Show();
        StartBattle = true;
    }
}
