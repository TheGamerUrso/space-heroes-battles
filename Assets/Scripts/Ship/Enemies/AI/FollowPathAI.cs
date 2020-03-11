using TheGamerUrso;
using UnityEngine;

public class FollowPathAI : BaseEnemyAI
{
    #region FollowPath AI Config
    [Header("FollowPath AI Config")]
    public int[] PathIndex;
    public Transform[] Path;
    public int currentPointToFollowIndex;

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
    protected float pathMagnitude;
    #endregion

    public void GeneratePath()
    {
        // Debug.Log("Generate new Path");
        curPath = Random.Range(0, PathIndex.Length);
        if (Waypoints.Instance == null)
        {
            return;
        }

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
        startingPosition = transform.position;

        if (Path.Length == 0)
        {
            GeneratePath();
        }
    }

    public override void Enter()
    {
        base.Enter();
        currentPointToFollowIndex = 0;
    }

    public override void Move()
    {
        Animator animator = enemy.GetAnimator();
        var info = animator.GetCurrentAnimatorStateInfo(0);

        if (info.shortNameHash == enterNameHash)
        {
            Debug.Log("Entering");
            return;
        }

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


            Vector3 direction = (newPos - transform.position);
            Vector3 normalizedDirection = direction.normalized;
            float distance = direction.magnitude;

            if (distance > 1)
            {
                rigid.MovePosition(transform.position + normalizedDirection * xVel * Time.deltaTime);
            }

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