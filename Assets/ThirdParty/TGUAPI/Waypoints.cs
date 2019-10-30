using UnityEngine;

public class Waypoints : MonoBehaviour
{
    public static Waypoints Instance;

    public GameObject[] ListOfPaths;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public GameObject GetPath(int waypointIndex)
    {
        return ListOfPaths[waypointIndex];
    }
}