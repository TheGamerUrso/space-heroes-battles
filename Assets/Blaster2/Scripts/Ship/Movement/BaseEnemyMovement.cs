using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyMovement : BaseMovementController
{
    protected Enemy enemy;
    protected bool Loop;
    protected Coroutine EnterCoroutine;

    protected Vector3 startingPosition;
    protected Vector3 dist;
    protected Vector2 MaxScreenBound;
    protected Vector2 MinScreenBound;
    protected float timer;
    protected float delay = .5f;
    protected Vector3 movement;

    public virtual void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    public virtual void Update()
    {
        if (enemy.enemyState == EnemyState.Combat)
        {
            Move();
        }
    }  

    public void CheckOutOfSight()
    {
        if (transform.position.z < Constants.m_ZMin)
        {
            gameObject.SetActive(false);
        }
    }
}
