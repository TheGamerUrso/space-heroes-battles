using System;
using System.Collections.Generic;
using System.Linq;
using Dan.Main;
using Dan.Models;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace TheGamerUrso
{
    public class Leaderboards : MonoSingleton<Leaderboards>
    {
        public Action<bool> OnNewEntryUploaded;
        public Action<Entry[]> OnLeaderboardValueChanged;
        public Action<bool> OnLoadingLeadeboard;
        public Action<string> OnErrorLoading;
        public Action<bool> OnUsernameUpdated;
        public Dan.Models.Entry[] LeaderboardEntries;
        //======================================================================================================================================================
        public static LeaderboardReference myLeaderbosard = new LeaderboardReference("85f0d99eba9f18f6538dc149ed9d3ee68bcb3215a97a38e075f16ad6e054ef9c");
        //======================================================================================================================================================
        [ContextMenu("Load")]
        public void LoadLeaderboard()
        {
            OnLoadingLeadeboard?.Invoke(true);
            myLeaderbosard.GetEntries(((entries) =>
            {
                LeaderboardEntries = entries;
                OnLeaderboardValueChanged?.Invoke(entries);
                OnLoadingLeadeboard?.Invoke(false);
            }), ErrorCallback);
        }

        //======================================================================================================================================================
        public void UploadNewEntry(int score)
        {
            OnLoadingLeadeboard?.Invoke(true);
            PlayerData playerData = PersistantData.GetPlayerData();

            if (score > (int)playerData.HighScore)
            {
                myLeaderbosard.UploadNewEntry(playerData.Username,score, (success) =>
                {
                    playerData.SetHighscore(score);
                    LoadLeaderboard();
                    OnNewEntryUploaded?.Invoke(success);
                }, ErrorCallback);
            }
            else
            {
                OnNewEntryUploaded?.Invoke(false);
            }
        }
        //======================================================================================================================================================
        private void ErrorCallback(string error)
        {
            Debug.LogError(error);
            OnLoadingLeadeboard?.Invoke(false);
            OnErrorLoading?.Invoke(error);
        }
        //======================================================================================================================================================
        public void SetUsername(string newUsername)
        {
            myLeaderbosard.UpdateEntryUsername(newUsername, (success) =>
            {
                if (success)
                {
                    PlayerData playerData = PersistantData.GetPlayerData();
                    playerData.SetUsername(newUsername);
                }
                OnUsernameUpdated?.Invoke(success);
            }, ErrorCallback);
        }
    }
}
