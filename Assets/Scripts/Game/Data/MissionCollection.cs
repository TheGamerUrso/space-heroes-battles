using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class MissionCollection
{
    public Mission[] Missions;
    public string MissionName;

    public Mission GetMission(int index)
    {
        return Missions[index];
    }

    public override string ToString()
    {
        string result = "Missions\n";
        foreach (var item in Missions)
        {
            result += string.Format("ID {0} - Mission: {1} \n Description: {2} \n\n", item.ID, item.Title, item.Description);
        }
        return result;
    }
}
