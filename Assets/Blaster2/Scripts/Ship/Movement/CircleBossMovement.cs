using UnityEngine;

public class CircleBossMovement : BossEnemyMovement
{
    [SerializeField] private float circleRadius = 3f;
    [SerializeField] private float angularSpeed = 2f;
    private float angle;

    protected override void OnCombatStarted()
    {
 
    }

    public override void Move()
    {
        angle += angularSpeed * Time.deltaTime;
        float x = entryTargetPosition.x + Mathf.Cos(angle) * circleRadius;
        float z = entryTargetPosition.z + Mathf.Sin(angle) * circleRadius;
        transform.position = new Vector3(x, transform.position.y, z);
    }
}
