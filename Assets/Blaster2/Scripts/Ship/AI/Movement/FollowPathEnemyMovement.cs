using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPathEnemyMovement : BaseEnemyMovement
{
    [Header("Movement")]
    public int[] PathIndex;
    public Transform[] Path;
    private int currentPointToFollowIndex;
    [Space()]
    [SerializeField] public bool PingPong = false;
    [SerializeField] protected bool RotateTowardDir = false;
    [SerializeField] public bool Reset = false;
    [SerializeField] protected float RotationSpeed;

    protected Vector3 targetRotation;

    [SerializeField] protected bool reverse;
    [SerializeField] protected bool Auto;

    private Vector3 newPos;
    private float step;
    private int curPath;
    private GameObject path;
    protected float pathMagnitude;

    public override void OnEnable()
    {
        currentPointToFollowIndex = 0;

        if (Path.Length > 0)
            transform.position = Path[0].position;

    }
    public override void Start()
    {
        startingPosition = transform.position;

        if (Path.Length == 0)
        {
            GeneratePath();
        }
    }
    public override void Movement()
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
                        Vector3 startDir = (startingPosition - transform.position).normalized;
                        transform.position += startDir * speed * Time.deltaTime;
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

            newPos.y = 0;

            Path[currentPointToFollowIndex].position = newPos;


            Vector3 direction = (newPos - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, newPos);

            if (distance > 1)
            {
                transform.position += direction * speed * Time.deltaTime;
            }

            if (RotateTowardDir)
            {
                step = RotationSpeed * Time.deltaTime;
                targetRotation = Vector3.Lerp(targetRotation, direction, step);
                transform.rotation = Quaternion.LookRotation(targetRotation, Vector3.up);
            }
        }
    }

    public void GeneratePathByIndex(int Index)
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

    public int GeneratePath()
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
    public void GeneratePath(Transform[] newPath)
    {
        Path = newPath;
    }
}
