using UnityEngine;

public class FollowPathEnemyMovement : BaseEnemyMovement
{
    [Header("Movement")]
    public int[] PathIndex;
    public Transform[] Path;
    private int currentPointToFollowIndex;
    [Space()]
    [SerializeField] public bool PingPong = false;
    [SerializeField] public bool Reset = false;
  

    [SerializeField] protected bool reverse;
    [SerializeField] protected bool Auto;

    private Vector3 newPos;
    private int curPath;
    private GameObject path;
    protected float pathMagnitude;

    public void OnEnable()
    {
        currentPointToFollowIndex = 0;

        if (Path.Length > 0)
            transform.position = Path[0].position;
    }

    public override void Setup(Vector3 spawnPos,Quaternion targetRotation)
    {
        int pathIndex = GeneratePath();

        if (enemy.GameObjectType == PoolGameObjectType.Enemy4)
        {
            PingPong = false;
            if (pathIndex == 0)
            {
                PingPong = true;
            }
        }
    }

    public void Start()
    {
        startingPosition = transform.position;

        if (Path.Length == 0)
        {
            GeneratePath();
        }
    }

    public override void Move()
    {
        if (Path == null || Path.Length == 0) return;

        Vector3 targetPos = Path[currentPointToFollowIndex].position;
        targetPos.y = 0; // Keep movement locked on the 2D plane if needed

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
                    enemy.Exit();
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

        if (currentPointToFollowIndex >= Path.Length)
        {
            currentPointToFollowIndex = Path.Length - 1;
        }

        currentPointToFollowIndex = Mathf.Clamp(currentPointToFollowIndex, 0, Path.Length - 1);
        targetPos = Path[currentPointToFollowIndex].position;
        targetPos.y = 0;


        Vector3 direction = (newPos - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, newPos);

        if (distance > 1)
        {
            transform.position += direction * speed * Time.deltaTime;
        }

        RotateTowardDirection(newPos);
    }

    private void GeneratePathByIndex(int Index)
    {
        curPath = Index;

        PingPong = false;
        if (curPath == 0)
        {
            PingPong = true;
        }

        if (Waypoints.Instance == null)
        {
            return;
        }

        path = Waypoints.Instance.GetPath(PathIndex[curPath]);
        Transform[] PathList = path.transform.GetChildrenAsList();

        GeneratePath(PathList);


        currentPointToFollowIndex = 0;
        Reset = false;
        Auto = true;

        transform.position = Path[0].position;
    }

    private int GeneratePath()
    {
        curPath = Random.Range(0, PathIndex.Length);

        if (Waypoints.Instance == null)
        {
            return -1;
        }

        path = Waypoints.Instance.GetPath(PathIndex[curPath]);
        Transform[] PathList = path.transform.GetChildrenAsList();

        GeneratePath(PathList);


        currentPointToFollowIndex = 0;
        Reset = false;
        Auto = true;

        transform.position = Path[0].position;
        return curPath;
    }

    private void GeneratePath(Transform[] newPath)
    {
        Path = newPath;
    }
}
