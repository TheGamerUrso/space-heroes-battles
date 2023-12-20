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
    public BossDestroyablePart[] bossDestroyableParts;
    public override void Awake()
    {
        base.Awake();
        targetPosition = transform.position;
    }

    public override void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);
    }
    public override void LateUpdate()
    {
        base.LateUpdate();
    }

    public override void Movement()
    {
        base.Movement();
    }

    public override void EnableMovement()
    {

    }
}


