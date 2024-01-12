using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TheGamerUrso.Models
{
    [Serializable]
    public class Entry
    {
        public int Rank;
        public string Username;
        public long Score;
        public Entry(int Rank, string Username, long Score)
        {
            this.Rank = Rank;
            this.Username = Username;
            this.Score = Score;
        }
        public bool IsMine()
        {
            if (Username == PersistantData.GetPlayerData().Username)
            {
                return true;
            }
            return false;
        }
    }
}
namespace TheGamerUrso
{
    public static class JsonSystem
    {
        [ContextMenu("Load Missions")]
        public static Leaderboard LoadLeaderboard()
        {
            var jsonTextFile = Resources.Load<TextAsset>("Data/LeaderboardData");

            try
            {
                return JsonUtility.FromJson<Leaderboard>(jsonTextFile.text);
            }
            catch (ArgumentException e)
            {
                Debug.LogError("Can't read File" + e.Message);
            }

            return null;
        }

        public static void SaveLeaderboard(Leaderboard leaderboard)
        {
            var LeaderboardPath = Application.dataPath + "/Resources/Data/leaderboardData.json";
            try
            {
                using (StreamWriter stream = new StreamWriter(LeaderboardPath))
                {
                    string json = JsonUtility.ToJson(leaderboard);
                    stream.Write(json);
                }
            }
            catch (ArgumentException e)
            {
                Debug.LogError("Can't read File" + e.Message);
            }
        }
    }
}
