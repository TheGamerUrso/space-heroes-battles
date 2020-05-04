using UnityEngine;

public class Waypoints : Singleton<Waypoints>
{
    public GameObject[] ListOfPaths;


    public GameObject GetPath(int waypointIndex)
    {
        return ListOfPaths[waypointIndex];
    }
}