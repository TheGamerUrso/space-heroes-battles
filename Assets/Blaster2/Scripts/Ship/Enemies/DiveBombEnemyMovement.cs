using UnityEngine;
using UnityEngine.Rendering;

public class DiveBombEnemyMovement : EnemyMovement
{
    [Header("Dive Bomb Configuration")]
    [SerializeField] private float diveSpeedMultiplier = 1.5f;
    [SerializeField] private float targetReachedThreshold = 0.5f;
    private Vector3 targetPosition;
    private bool hasTarget = false;
    private bool hasReachedTarget = false;

    private void OnEnable()
    {
        AcquirePlayerTarget();
    }

    public override void Setup(Vector3 spawnPos, Quaternion targetRotation)
    {
        base.Setup(spawnPos, targetRotation); 
        AcquirePlayerTarget();
    }

    private void AcquirePlayerTarget()
    {
        hasReachedTarget = false;

        // Query the player position via GameObject lookup (matches project pattern in GameController)
        var playerShip = GameObject.FindAnyObjectByType<PlayerShip>();
        if (playerShip != null)
        {
            targetPosition = playerShip.transform.position;
            hasTarget = true;
        }
        else
        {
            // Fallback default path if player reference is missing
            targetPosition = transform.position + (Vector3.down * 15f);
            hasTarget = true;
        }

        // Optionally orient the sprite toward the dive vector
        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction != Vector3.zero)
        {
                   transform.rotation = Quaternion.LookRotation(targetRotation, Vector3.up);
        }
    }

    public override void Move()
    {
        if (!hasTarget) return;

        if (CheckOutOfSight())
        {
            ((Enemy)ship).Exit();
            return;
        }

        if (!hasReachedTarget)
        {
            // Move rapidly toward the locked player coordinates
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                speed * diveSpeedMultiplier * Time.deltaTime
            );

            RotateTowardDirection(targetPosition);

            // Check if destination has been reached
            if (Vector3.Distance(transform.position, targetPosition) <= targetReachedThreshold)
            {
                hasReachedTarget = true;
                transform.rotation = Quaternion.LookRotation(transform.forward, Vector3.up);
            }
        }
        else
        {
            // Post-dive behavior: continue straight down off-screen to be cleaned up by object pooling
            transform.position += transform.forward * speed * Time.deltaTime;
        }     
    }
}
