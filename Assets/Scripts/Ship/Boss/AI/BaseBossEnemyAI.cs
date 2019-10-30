using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBossEnemyAI : FollowPathAI
{
    [Header("Boss AI Config")]
    [SerializeField] protected GameObject WaypointPrefab;
    [SerializeField] protected bool m_IsMovingVertical = false;

    protected Transform[] m_Waypoints = new Transform[3];
    [HideInInspector] public bool appear;
    [HideInInspector] public bool entered;
    protected GameObject waypointsGameObject;

    public override void Initialize()
    {
        List<Transform> newList = new List<Transform>();

        waypointsGameObject = Instantiate(WaypointPrefab, Vector3.zero, Quaternion.identity);
        waypointsGameObject.transform.SetParent(transform.parent);

        for (int i = 0; i < waypointsGameObject.transform.childCount; i++)
        {
            newList.Add(waypointsGameObject.transform.GetChild(i));
        }

        GeneratePath(newList.ToArray());

    }

    public void ChangeWaypointByIndex(int currentPointToFollowIndex)
    {
        this.currentPointToFollowIndex = currentPointToFollowIndex;
    }

    public virtual void ChangeWaypoint(int hitIndex)
    {
        if (!m_IsMovingVertical)
        {
            m_IsMovingVertical = true;
            StartCoroutine(MoveVerticalWithDelay(hitIndex));
        }
    }

    private IEnumerator MoveVerticalWithDelay(int hitIndex)
    {
        if (currentPointToFollowIndex == 1)
        {
            currentPointToFollowIndex = 2;
        }
        else if (currentPointToFollowIndex == 2)
        {
            currentPointToFollowIndex = 1;
        }
        else
        {
            currentPointToFollowIndex = Random.Range(1, Path.Length);
        }

        yield return new WaitForSeconds(1);
        m_IsMovingVertical = false;
        hitIndex = 0;
    }
}
