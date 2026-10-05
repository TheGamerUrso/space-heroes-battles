using UnityEngine;

public class StationaryStrafeMovement : BaseEnemyMovement
{
    [Header("Sentry Configuration")]
    [SerializeField] private float targetStopY = 3.0f; // The Y coordinate to stop at
    [SerializeField] private float strafeSpeed = 3.5f;
    [SerializeField] private float changeDirectionInterval = 2f;

    private bool hasReachedPosition = false;
    private float directionTimer;
    private int currentDirection = 0; // -1 for left, 1 for right, 0 for idle

    public override void Move()
    {
        // Check if out of bounds (off-screen)
        if (CheckOutOfSight())
        {
            if (ship is Enemy enemy)
            {
                enemy.SetState(EnemyState.Escape);
            }
            return;
        }

        if (!hasReachedPosition)
        {
            // Phase 1: Enter screen and move down until target Y is reached
            transform.position += Vector3.down * Speed * Time.deltaTime;

            if (transform.position.y <= targetStopY)
            {
                // Snap to exact Y position and transition to combat state
                transform.position = new Vector3(transform.position.x, targetStopY, transform.position.z);
                hasReachedPosition = true;

                // Optional: If you want to trigger weapon firing explicitly upon arrival, 
                // you can notify your enemy/weapon controller here.
            }
        }
        else
        {
            // Phase 2: Reached destination, start random left/right strafing
            directionTimer -= Time.deltaTime;
            if (directionTimer <= 0)
            {
                directionTimer = Random.Range(1f, changeDirectionInterval);
                PickRandomDirection();
            }

            // Apply horizontal strafe movement
            Vector3 strafeMovement = Vector3.right * currentDirection * strafeSpeed * Time.deltaTime;
            transform.position += strafeMovement;

            // Clamp X position using your Constants class to keep them inside the play area
            float clampedX = Mathf.Clamp(transform.position.x, Constants.m_XMin, Constants.m_XMax);
            transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
        }
    }

    private void PickRandomDirection()
    {
        int rand = Random.Range(0, 3);
        if (rand == 0)
        {
            currentDirection = -1; // Move Left
        }
        else if (rand == 1)
        {
            currentDirection = 1;  // Move Right
        }
        else
        {
            currentDirection = 0;  // Pause/Hold position briefly
        }
    }
}
