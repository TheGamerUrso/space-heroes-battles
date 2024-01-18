using UnityEngine;
using System.Collections;
using Dan.Main;
using System;
using Dan.Models;

public class LeaderboardScreen : MonoBehaviour
{
    [SerializeField] private CanvasGroup _leaderboardLoadingPanel;

     [SerializeField] private GameObject LeaderboardEntry;
     [SerializeField] private GameObject PlayerLeaderboardEntry;
     [SerializeField] private Transform content;
     [SerializeField] private GameObject errorMessage;

    //======================================================================================================================================================
    void Start()
    {
        PlayerData playerData = PersistantData.GetPlayerData();
        PlayerLeaderboardEntry.GetComponent<LeaderBoardEntry>().UserNameText.SetText(playerData.GetUsername());
        PlayerLeaderboardEntry.GetComponent<LeaderBoardEntry>().ScoreText.SetText(playerData.HighScore.ToString());
    }

    //======================================================================================================================================================
    public void LoadLeaderboard()
    {
        PlayerData playerData = PersistantData.GetPlayerData();
        ToggleLoadingPanel(true);
        PlayerLeaderboardEntry.GetComponent<LeaderBoardEntry>().RankText.SetText("-");
        for (int i = 0; i < TheGamerUrso.Leaderboards.Instance.LeaderboardEntries.Length; i++)
        {
            Entry entry = TheGamerUrso.Leaderboards.Instance.LeaderboardEntries[i];
            if (entry.IsMine())
            {
                PlayerLeaderboardEntry.GetComponent<LeaderBoardEntry>().RankText.SetText(entry.RankSuffix());
                break;
            }

        }
        PlayerLeaderboardEntry.GetComponent<LeaderBoardEntry>().UserNameText.SetText(playerData.GetUsername());
        PlayerLeaderboardEntry.GetComponent<LeaderBoardEntry>().ScoreText.SetText(playerData.HighScore.ToString());
        OnLeaderboardLoaded(TheGamerUrso.Leaderboards.Instance.LeaderboardEntries);
    }

    //======================================================================================================================================================
    private void OnLeaderboardLoaded(Dan.Models.Entry[] entries)
    {
        errorMessage.SetActive(false);
        foreach (Transform t in content)
            Destroy(t.gameObject);

        foreach (var t in entries)
            CreateEntryDisplay(t);

        ToggleLoadingPanel(false);
    }
    //======================================================================================================================================================
    private void CreateEntryDisplay(Dan.Models.Entry entry)
    {
        GameObject leaderboardGO = Instantiate(LeaderboardEntry, content);
        leaderboardGO.GetComponent<LeaderBoardEntry>().SetEntry(entry);
    }
    //======================================================================================================================================================
    private void ToggleLoadingPanel(bool isOn)
    {
        _leaderboardLoadingPanel.alpha = isOn ? 1f : 0f;
        _leaderboardLoadingPanel.interactable = isOn;
        _leaderboardLoadingPanel.blocksRaycasts = isOn;
    }

    private void OnErrorLoadingCallback(string error)
    {
        errorMessage.SetActive(true);
    }
}




