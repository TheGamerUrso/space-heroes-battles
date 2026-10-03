using UnityEngine;

public class FollowPathEnemyMovement : BaseEnemyMovement
{
    [Header("Movement")]
    public int[] Paths;
    private int PathIndex;

    public Transform[] Path;
    private int currentPointToFollowIndex;
    [Space()]
    [SerializeField] public bool PingPong = false;
    [SerializeField] public bool Reset = false;
  

    [SerializeField] protected bool reverse;
    [SerializeField] protected bool Auto;

    protected float pathMagnitude;

    public override void Setup(Vector3 spawnPos,Quaternion targetRot)
    {
        GeneratePath();

        startingPosition = Path[currentPointToFollowIndex].transform.position;
        transform.position = Path[currentPointToFollowIndex].transform.position;
        transform.rotation = Quaternion.identity;

        RotateTowardDirection(transform.forward);

        if (((Enemy)ship).GameObjectType == PoolGameObjectType.Enemy4)
        {
            PingPong = false;
            if (PathIndex == 0)
            {
                PingPong = true;
            }
        }
    }

    public override void Move()
    {
        if (Path == null || Path.Length == 0) return;

         if (currentPointToFollowIndex >= Path.Length)
        {
            currentPointToFollowIndex = Path.Length - 1;
        }

        Vector3 targetPos = Path[currentPointToFollowIndex].position;
        Vector3 direction = (targetPos - transform.position).normalized;

        pathMagnitude = Vector3.Distance(transform.position, targetPos);

        if (pathMagnitude < 2f)
        {
            if (currentPointToFollowIndex >= Path.Length - 1)
            {
                if (Reset && !PingPong)
                {
                    currentPointToFollowIndex = 0;
                }
                else if (!Reset && PingPong)
                {
                    reverse = true;
                }
                else if (!Reset && !PingPong)
                {
                    ship.Exit();
                    return;
                }
            }
            else if (currentPointToFollowIndex <= 0)
            {
                reverse = false;
            }

            if (Auto)
            {
                currentPointToFollowIndex += reverse ? -1 : 1;
            }
        } 
      
        if (pathMagnitude > 1)
        {
            transform.position += direction * speed * Time.deltaTime;
        }

        RotateTowardDirection(targetPos);
    }

    private void GeneratePath()
    {
        if (Waypoints.Instance == null) return;
        var waypoints = Waypoints.Instance.GetPath(Paths[PathIndex]);
        Path = waypoints.transform.GetChildrenAsList(); 
        currentPointToFollowIndex = 0;
        Reset = false;
        Auto = true;
    }
}
