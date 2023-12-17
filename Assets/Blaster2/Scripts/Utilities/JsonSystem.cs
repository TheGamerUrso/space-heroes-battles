using System;
using System.IO;
using UnityEngine;

public static class JsonSystem
{
    private static string MissionsPath;
    private static string LevelObjectivePath;
    [ContextMenu("Load Missions")]
    public static MissionCollection LoadMissions()
    {
        var jsonTextFile = Resources.Load<TextAsset>("Data/MissionBriefingData");

        try
        {
            return JsonUtility.FromJson<MissionCollection>(jsonTextFile.text);
        }
        catch (ArgumentException e)
        {
            Debug.LogError("Can't read File" + e.Message);
        }

        return null;
    }

    public static void SaveMissionBriefingData(MissionCollection missionCollection)
    {
        MissionsPath = Application.dataPath + "/Resources/Data/MissionBriefingData.json";
        try
        {
            using (StreamWriter stream = new StreamWriter(MissionsPath))
            {
                string json = JsonUtility.ToJson(missionCollection);
                stream.Write(json);
            }
        }
        catch (ArgumentException e)
        {
           Debug.LogError("Can't read File" + e.Message);
        }
    }
}
