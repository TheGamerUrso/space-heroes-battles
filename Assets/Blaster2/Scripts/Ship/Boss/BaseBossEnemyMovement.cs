using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBossEnemyMovement : BaseEnemyMovement
{
    [Header("Movement")]
    [SerializeField] protected Vector3[] Positions;
    [SerializeField] protected int currentPos;
    [SerializeField] protected float changePositionTimer;
    protected Vector3 targetPosition;
    protected bool RandomMovement;
    
    public void OnDestroy()
    {
        enemy.GetComponent<BossEnemy>().OnBossPhaseChanged-=OnBossPhaseChangedHandled;
    }

    public override void Awake()
    {
        base.Awake();
        targetPosition = transform.position;
        enemy.GetComponent<BossEnemy>().OnBossPhaseChanged+=OnBossPhaseChangedHandled;

    }
    
    public override void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);
    }

    public override void Move()
    {
        base.Move();
    }

    public virtual void OnBossPhaseChangedHandled(int Phase){}
}


