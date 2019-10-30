using TheGamerUrso;
using UnityEngine;

public class FollowPathAI : BaseEnemyAI
{
    [Header("FollowPath AI Config")]
    public int[] PathIndex;
    public Transform[] Path;
    [HideInInspector] public int currentPointToFollowIndex;

    [SerializeField] protected bool PingPong = false;
    [SerializeField] protected bool RotateTowardDir = false;
    [SerializeField] public bool Reset = false;
    [SerializeField] protected float RotationSpeed;

    protected Vector3 targetRotation;

    [SerializeField] protected bool reverse;
    [SerializeField] protected bool SpawnAtFirstPath;
    [SerializeField] protected bool Auto;
    private Vector3 newPos;
    private float step;
    private int curPath;
    private Vector3 dir;
    private Transform[] PathList;
    private GameObject path;
    private float pathMagnitude;

    public void GeneratePath()
    {
        Debug.Log("Generate new Path");
        curPath = Random.Range(0, PathIndex.Length);
        path = Waypoints.Instance.GetPath(PathIndex[curPath]);
        Transform[] PathList = TransformExtention.GetChildrenAsList(path.transform);
        GeneratePath(PathList);

        currentPointToFollowIndex = 0;
        Reset = false;
        Auto = true;

        if (SpawnAtFirstPath)
        {
            transform.localPosition = Path[0].position;
        }
    }


    public void GeneratePath(Transform[] newPath)
    {
        Path = newPath;
    }

    public override void Initialize()
    {
        if (Path.Length == 0)
        {
            GeneratePath();
        }
        startingPosition = transform.localPosition;

    }
    public override void Enter()
    {
        transform.localPosition = startingPosition;
        currentPointToFollowIndex = 0;
    }

    public override void Move()
    {
        if (Path.Length > 0)
        {
            pathMagnitude = (Path[currentPointToFollowIndex].position - transform.position).magnitude;
            if (pathMagnitude < 2)
            {
                if (currentPointToFollowIndex == Path.Length - 1)
                {
                    if (Reset && !PingPong)
                    {
                        rigid.MovePosition(startingPosition);
                        //transform.position = startingPosition;
                        currentPointToFollowIndex = 0;
                    }
                    else if (!Reset && PingPong)
                    {
                        reverse = true;
                    }
                    else if (!Reset && !PingPong)
                    {
                        Leave();
                    }
                }
                else if (currentPointToFollowIndex == 0)
                {
                    reverse = false;
                }

                if (Auto)
                {
                    if (reverse)
                    {
                        currentPointToFollowIndex--;
                    }
                    else if (reverse == false)
                    {
                        currentPointToFollowIndex++;
                    }
                }
            }

            if (currentPointToFollowIndex >= Path.Length)
            {
                currentPointToFollowIndex = Path.Length - 1;
            }

            newPos = Path[currentPointToFollowIndex].position;

            newPos.y = -50;

            Path[currentPointToFollowIndex].position = newPos;

            rigid.MovePosition(Vector3.MoveTowards(transform.position, newPos, XVel * Time.deltaTime));

            if (RotateTowardDir)
            {
                step = RotationSpeed * Time.deltaTime;
                dir = (transform.position - newPos).normalized;
                targetRotation = Vector3.Lerp(targetRotation, dir, step);
                rigid.MoveRotation(Quaternion.LookRotation(targetRotation, Vector3.up));
            }
        }
    }

}