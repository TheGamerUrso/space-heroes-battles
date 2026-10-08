using UnityEngine;

public class StrafeBossMovement : BossEnemyMovement
{
    [SerializeField] private float strafeSpeed = 4f;
    [SerializeField] private float strafeWidth = 5f;
    private float initialX;
    private float targetX;
    private float combatStartTime; // Track when combat begins
    private bool hasInitialized;

    public override void Move()
    {
        // Capture initial position and time on the very first frame movement starts
        if (!hasInitialized)
        {
            initialX = transform.position.x;
            combatStartTime = Time.time;
            hasInitialized = true;

            targetPosition = new Vector3(initialX, transform.position.y,
         transform.position.z);
        }

        float combatTime = Time.time - combatStartTime;

        targetX = initialX + Mathf.Sin(combatTime * strafeSpeed) * strafeWidth;
        targetPosition = new Vector3(targetX,
            transform.position.y,
            transform.position.z);

        transform.position = Vector3.Lerp(transform.position,
            targetPosition, speed * Time.deltaTime);
    }
}
