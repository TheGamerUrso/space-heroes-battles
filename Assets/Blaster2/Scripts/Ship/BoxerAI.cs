using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
public class BoxerAI : BaseBossEnemyAI
{
    public Punch[] punches;
    public bool attacking;
    public GameObject ShipPivot;
    public Ease ease;
    public float speed;

    public override void Start()
    {
        base.Start();
        cooldown = UnityEngine.Random.Range(4, 8);

        Sequence seq = DOTween.Sequence();
        seq.Append(ShipPivot.transform.DOMoveX(-1, speed).SetEase(ease));
        seq.Append(ShipPivot.transform.DOMoveX(1, speed).SetEase(ease));
        seq.Append(ShipPivot.transform.DOMoveX(0, speed).SetEase(ease));
        seq.SetLoops(-1).Play();
    }

    public override void Move()
    {
        if (baseBoss.CurrentHealth > 0)
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
                        newList[rand].Attack((x) => { attacking = x;
                        });
                    }
                }
            }
        }
    }

}
