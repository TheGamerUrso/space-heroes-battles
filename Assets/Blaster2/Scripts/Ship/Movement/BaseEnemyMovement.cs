using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class BaseEnemyMovement : BaseMovementController
{
   [SerializeField] protected Enemy enemy;
    protected bool Loop;
    protected Coroutine EnterCoroutine;

    protected Vector3 startingPosition;
    protected Vector3 startingRotation;
    protected Vector3 dist;
    protected Vector2 MaxScreenBound;
    protected Vector2 MinScreenBound;
    protected float timer;
    protected float delay = .5f;
    protected Vector3 movement;
    private float step;
    protected Vector3 targetRotation;
    [SerializeField] protected float RotationSpeed = 2;
    [SerializeField] protected bool RotateTowardDir = true;

    public virtual void Update()
    {
        if (enemy.enemyState == EnemyState.Combat)
        {
            Move();
        }
    }
    public virtual void Setup(Vector3 spawnPos,Quaternion targetRotaiton)
    {
        targetRotation = Vector3.zero;
        startingPosition = spawnPos;
        transform.position = spawnPos;
        transform.rotation = targetRotaiton;

        RotateTowardDirection(transform.forward);
    }

    public bool CheckOutOfSight()
    {
        if (transform.position.z < Constants.m_ZMin)
        {
            return true;
        }
        return false;
    }
   public void RotateTowardDirection(Vector3 targetPosition)
    {
        if (!RotateTowardDir) return;

        var direction = (targetPosition - transform.position).normalized;

        if (direction == Vector3.zero) return;

        // Smoothly interpolate toward the target direction
        step = RotationSpeed * Time.deltaTime;
        targetRotation = Vector3.Lerp(targetRotation, direction, step);

        // Apply rotation (Ensure your 2D/3D sprite orientation aligns with Z-forward LookRotation)
        transform.rotation = Quaternion.LookRotation(targetRotation, Vector3.up);
    }
}
