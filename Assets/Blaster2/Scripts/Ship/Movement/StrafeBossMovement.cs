using UnityEngine;

public class StrafeBossMovement : BossEnemyMovement
{
    [SerializeField] private float strafeSpeed = 4f;
    [SerializeField] private float strafeWidth = 5f;
    private float initialX;

    protected override void OnCombatStarted()
    {
        initialX = transform.position.x;
    }

    public override void Move()
    {
        float x = initialX + Mathf.Sin(Time.time * strafeSpeed) * strafeWidth;
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }
}
