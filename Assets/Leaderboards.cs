using System;
using System.Collections.Generic;
using System.Linq;
using Dan.Main;
using Dan.Models;
using TMPro;
using UnityEngine;

namespace TheGamerUrso
{
    [Serializable]
    public class Leaderboard
    {
        public List<TheGamerUrso.Models.Entry> entries = new List<TheGamerUrso.Models.Entry>();
        public TheGamerUrso.Models.Entry GetLeaderboard(int index)
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
        public TheGamerUrso.Models.Entry GetLeaderboardEntry(string username)
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
        public void Add(TheGamerUrso.Models.Entry newEntry)
        {
            TheGamerUrso.Models.Entry foundEntry = GetLeaderboardEntry(newEntry.Username);
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

    public class Leaderboards : MonoSingleton<Leaderboards>
    {
        public Action<Entry[]> OnLeaderboardValueChanged;
        public Action<bool> OnLoadingLeadeboard;
        public Action<string> OnErrorLoading;
        public static string UserName;

        //======================================================================================================================================================
        public static LeaderboardReference myLeaderbosard = new LeaderboardReference("85f0d99eba9f18f6538dc149ed9d3ee68bcb3215a97a38e075f16ad6e054ef9c");

        [SerializeField] private Leaderboard leaderboard;
        public long Score;

        [SerializeField] private int _defaultPageNumber = 1, _defaultEntriesToTake = 100;
        public float requestTimer;

        public List<Dan.Models.Entry> Entries = new List<Entry>();

        public bool DebugMode;
        //======================================================================================================================================================
        public void Start()
        {
            if (leaderboard == null)
            {
                leaderboard = new Leaderboard();
            }

            LoadLeaderboard();
        }
        void Update()
        {
            requestTimer -= Time.deltaTime;
            if (requestTimer <= 0)
            {
                requestTimer = 0;
            }
        }

        //======================================================================================================================================================
        public void AddScore(int Score)
        {
            if (LeaderboardContainsEntry(TheGamerUrso.Leaderboards.UserName))
            {
                var entry = GetLeaderboardEntry(TheGamerUrso.Leaderboards.UserName);
                entry.Score = Score;
            }
            leaderboard.Add(new TheGamerUrso.Models.Entry(0, TheGamerUrso.Leaderboards.UserName, Score));

        }
        //======================================================================================================================================================
        public TheGamerUrso.Models.Entry GetLeaderboardEntry(string username)
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
        public List<TheGamerUrso.Models.Entry> GetEntries()
        {
            return leaderboard.entries;
        }
        //======================================================================================================================================================
        [ContextMenu("Load")]
        public void LoadLeaderboard()
        {
            OnLoadingLeadeboard?.Invoke(true);
            var timePeriod = Dan.Enums.TimePeriodType.ThisMonth;

            var pageNumber = _defaultPageNumber;
            pageNumber = Mathf.Max(1, pageNumber);

            var take = _defaultEntriesToTake;
            take = Mathf.Clamp(take, 1, 100);

            var searchQuery = new Dan.Models.LeaderboardSearchQuery
            {
                Skip = (pageNumber - 1) * take,
                Take = take,
                TimePeriod = timePeriod
            };

            if (requestTimer > 0) return;
            requestTimer = 30;


            if (DebugMode)
            {
                OnLeaderboardLoaded(Entries.ToArray());
            }
            else
            {
                myLeaderbosard.GetEntries(searchQuery, OnLeaderboardLoaded, ErrorCallback);
                TheGamerUrso.Leaderboards.myLeaderbosard.GetPersonalEntry(OnPersonalEntryLoaded);
            }

            //leaderboard = JsonSystem.LoadLeaderboard();
            //OnLoadingLeadeboard?.Invoke(false);
        }
        //======================================================================================================================================================
        [ContextMenu("Save")]
        public void SaveLeaderboard()
        {
            //JsonSystem.SaveLeaderboard(leaderboard);
            PlayerData playerData = PersistantData.GetPlayerData();
            Instance.AddScore((int)playerData.HighScore);
        }
        //======================================================================================================================================================
        [ContextMenu("Add")]
        public void AddScore()
        {
            Submit();

            // if (string.IsNullOrEmpty(newEntry.Username) || newEntry.Score <= 0) return;

            //leaderboard.Add(new TheGamerUrso.Models.Entry(newEntry.Rank, newEntry.Username, newEntry.Score));
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

        //======================================================================================================================================================
        private void OnLeaderboardLoaded(Dan.Models.Entry[] entries)
        {
            Entries = entries.ToList();
            OnLeaderboardValueChanged?.Invoke(entries);
            OnLoadingLeadeboard?.Invoke(false);
        }
        //======================================================================================================================================================
        public void Submit()
        {
            bool shouldUpload = true;
            OnLoadingLeadeboard?.Invoke(true);
            PlayerData playerData = PersistantData.GetPlayerData();

            for (int i = 0; Entries.Count < 0; i++)
            {
                Dan.Models.Entry foundEntry = Entries[i];
                if (Entries[i].Username == UserName)
                {
                    if ((int)playerData.HighScore > Entries[i].Score)
                    {
                        foundEntry.Score = (int)playerData.HighScore;
                    }
                    shouldUpload = false;
                    break;
                }
            }

            if (!shouldUpload)return;

            if (DebugMode)
            {
                Callback(true);
            }
            else
            {
                    myLeaderbosard.UploadNewEntry(UserName, (int)playerData.HighScore, Callback, ErrorCallback);
            }
        }
        //======================================================================================================================================================
        public void DeleteEntry()
        {
            if (requestTimer > 0) return;
            requestTimer = 30;
            OnLoadingLeadeboard?.Invoke(true);
            if (DebugMode)
            {
                Callback(true);
            }
            else
            {
                myLeaderbosard.DeleteEntry(Callback, ErrorCallback);
            }
        }
        //======================================================================================================================================================
        public void ResetPlayer()
        {
            if (requestTimer > 0) return;
            requestTimer = 30;

            if (DebugMode) return;
            LeaderboardCreator.ResetPlayer();
        }

        //======================================================================================================================================================

        public void GetPersonalEntry(Action<Dan.Models.Entry> OnPersonalEntryLoaded)
        {
            OnLoadingLeadeboard?.Invoke(true);
            myLeaderbosard.GetPersonalEntry(OnPersonalEntryLoaded, ErrorCallback);
        }

        [ContextMenu("Get Personal Entry")]
        public void GetPersonalEntry()
        {
            myLeaderbosard.GetPersonalEntry(OnPersonalEntryLoaded, ErrorCallback);
        }

        public void OnPersonalEntryLoaded(Dan.Models.Entry entry)
        {
            OnLoadingLeadeboard?.Invoke(true);
            if (entry.Username != "Unknown")
                UserName = entry.Username;
        }

        //======================================================================================================================================================
        private void Callback(bool success)
        {
            if (success)
            {
                LoadLeaderboard();
            }
            else
            {
                OnErrorLoading?.Invoke("");
            }
        }
        //======================================================================================================================================================
        private void ErrorCallback(string error)
        {
            Debug.LogError(error);
            OnLoadingLeadeboard?.Invoke(false);
            OnErrorLoading?.Invoke(error);
        }

        public void SetUsername(string newUsername,Action<bool> OnUsernameUpdated)
        {
            UserName = newUsername;

            if (DebugMode)
            {
                Callback(true);
                OnUsernameUpdated(true);
            }
            else
            {        
                if (requestTimer > 0) return;
                requestTimer = 30;
                myLeaderbosard.UpdateEntryUsername(newUsername, OnUsernameUpdated, ErrorCallback);
            }
        }

        public string GetUsername()
        {
            if (string.IsNullOrEmpty(UserName))
            {
                string uniqueNumber = Guid.NewGuid().ToString();
                var newString = uniqueNumber.Substring(0, 4);
                UserName = $"Player#{newString}";
            }
            return UserName;
        }
    }
}
