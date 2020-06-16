using UnityEngine;

public class Waypoints : MonoSingleton<Waypoints>
{
    public GameObject[] ListOfPaths;

    public GameObject GetPath(int waypointIndex)
    {
        return ListOfPaths[waypointIndex];
    }
}