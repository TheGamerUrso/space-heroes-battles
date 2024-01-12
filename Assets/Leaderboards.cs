using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static JsonSystem;

public class Leaderboards : MonoSingleton<Leaderboards>
{
    [Serializable]
    public class Leaderboard
    {
        public List<Entry> entries = new List<Entry>();
        public Entry GetLeaderboard(int index)
        {
            return entries[index];
        }

        public override string ToString()
        {
            string result = "Missions\n";
            foreach (var entry in entries)
            {
                result += string.Format("Rank {0} - Username: {1} \n Score: {2} \n\n", entry.Rank, entry.Username, entry.Score);
            }
            return result;
        }
        public Entry GetLeaderboardEntry(string username)
        {
            foreach (var entry in entries)
            {
                if (entry.Username.Equals(username))
                {
                    return entry;
                }
            }
            return null;
        }
        public void Add(Entry newEntry)
        {
            Entry foundEntry = GetLeaderboardEntry(newEntry.Username);
            if (foundEntry == null)
            {
                entries.Add(newEntry);
            }
            else
            {
                foundEntry.Score = newEntry.Score;
            }

            var sortedList = entries.OrderByDescending(x => x.Score).ToList();
            entries = sortedList;
            for (int i = 0; i < entries.Count; i++)
            {
                entries[i].Rank = i + 1;
            }

            JsonSystem.SaveLeaderboard(this);
        }
    }
    //======================================================================================================================================================
    [SerializeField] private Leaderboard leaderboard;
    public Entry newEntry;
    //======================================================================================================================================================
    public void Start()
    {
        LoadLeaderboard();
        if (leaderboard == null)
        {
            leaderboard = new Leaderboard();
        }
    }

    //======================================================================================================================================================
    public void AddScore(long Score)
    {
        var username = PersistantData.GetPlayerData().Username;
        if (LeaderboardContainsEntry(username))
        {
            var entry = GetLeaderboardEntry(username);
            entry.Score = Score;
        }
        leaderboard.Add(new Entry(0, username, Score));

    }
    //======================================================================================================================================================
    public Entry GetLeaderboardEntry(string username)
    {
        foreach (var entry in leaderboard.entries)
        {
            if (entry.Username.Equals(username))
            {
                return entry;
            }
        }
        return null;
    }

    public bool LeaderboardContainsEntry(string username)
    {
        foreach (var entry in leaderboard.entries)
        {
            if (entry.Username.Equals(username))
            {
                return true;
            }
        }
        return false;
    }
    //======================================================================================================================================================
    public List<Entry> GetEntries()
    {
        return leaderboard.entries;
    }
    //======================================================================================================================================================
    [ContextMenu("Load")]
    public void LoadLeaderboard()
    {
        leaderboard = JsonSystem.LoadLeaderboard();
    }
    //======================================================================================================================================================
    [ContextMenu("Save")]
    public void SaveLeaderboard()
    {
        JsonSystem.SaveLeaderboard(leaderboard);
    }
    //======================================================================================================================================================
    [ContextMenu("Add")]
    public void AddScore()
    {
        if (string.IsNullOrEmpty(newEntry.Username) || newEntry.Score <= 0) return;

        leaderboard.Add(new Entry(newEntry.Rank, newEntry.Username, newEntry.Score));
    }
    //======================================================================================================================================================
    [ContextMenu("Order By Score")]
    public void OrderByRank()
    {
        var sortedList = leaderboard.entries.OrderByDescending(x => x.Score).ToList();
        leaderboard.entries = sortedList;
        for (int i = 0; i < leaderboard.entries.Count; i++)
        {
            leaderboard.entries[i].Rank = i + 1;
        }
    }
}
