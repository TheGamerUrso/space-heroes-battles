using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Leaderboards : MonoSingleton<Leaderboards>
{
    public class Entry
    {
        public int Rank;
        public string Username;
        public int Score;

        public bool IsMine()
        {
            if (Username == PersistantData.GetPlayerData().Username)
            {
                return true;
            }
            return false;
        }
    }
    public Dictionary<string, Entry> LeaderboardTable = new Dictionary<string, Entry>();
   //======================================================================================================================================================
    public void AddScore(int Score)
    {
        var username = PersistantData.GetPlayerData().Username;
        if (LeaderboardTable.ContainsKey(username))
        {
            LeaderboardTable[username].Score = Score;
        }
        LeaderboardTable.Add(username, new Entry() { Rank = LeaderboardTable.Count, Username = username, Score = Score });
    }
   //======================================================================================================================================================
    public List<Entry> GetEntries()
    {
        List<Entry> entries = new List<Entry>();
        foreach (KeyValuePair<string, Entry> keyValuePair in LeaderboardTable)
        {
            entries.Add(keyValuePair.Value);
        }
        return entries;
    }
   //======================================================================================================================================================
    public void LoadLeaderboard()
    {
        
    }
    //======================================================================================================================================================
    public void SaveLeaderboard()
    {
        var jsonTextToSave = "";

        foreach (KeyValuePair<string, Entry> keyValuePair in LeaderboardTable)
        {
            jsonTextToSave += keyValuePair.Value.Rank + "," + keyValuePair.Value.Username + "," + keyValuePair.Value.Score + "\n";
        }
        JsonUtility.ToJson(jsonTextToSave);
    }
}
