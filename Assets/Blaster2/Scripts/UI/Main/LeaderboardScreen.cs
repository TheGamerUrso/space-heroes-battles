using UnityEngine;
using System.Collections;

public class LeaderboardScreen : MonoBehaviour
{
    [SerializeField] private CanvasGroup _leaderboardLoadingPanel;

    public GameObject LeaderboardEntry;
    public GameObject PlayerLeaderboardEntry;
    public Transform content;
    public string Username;

    void OnDestroy()
    {
        if (TheGamerUrso.Leaderboards.Instance != null)
            TheGamerUrso.Leaderboards.Instance.OnLeaderboardValueChanged -= OnLeaderboardLoaded;
    }

    //======================================================================================================================================================
    void Start()
    {
        TheGamerUrso.Leaderboards.Instance.OnLeaderboardValueChanged += OnLeaderboardLoaded;
        Username = PersistantData.GetPlayerData().GetUsername();
        PlayerData playerData = PersistantData.GetPlayerData();
        PlayerLeaderboardEntry.GetComponent<LeaderBoardEntry>().RankText.SetText("-");
        PlayerLeaderboardEntry.GetComponent<LeaderBoardEntry>().UserNameText.SetText(playerData.Username);
        PlayerLeaderboardEntry.GetComponent<LeaderBoardEntry>().ScoreText.SetText(playerData.HighScore.ToString());
        TheGamerUrso.Leaderboards.Instance.LoadLeaderboard();
    }

    //======================================================================================================================================================
    private void OnLeaderboardLoaded(Dan.Models.Entry[] entries)
    {
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
        if (entry.IsMine())
        {
            PlayerLeaderboardEntry.GetComponent<LeaderBoardEntry>().RankText.SetText("" + entry.Rank);
            PlayerLeaderboardEntry.GetComponent<LeaderBoardEntry>().UserNameText.SetText(Username);
            PlayerLeaderboardEntry.GetComponent<LeaderBoardEntry>().ScoreText.SetText("" + entry.Score);
        }
    }
    //======================================================================================================================================================
    private void ToggleLoadingPanel(bool isOn)
    {
        _leaderboardLoadingPanel.alpha = isOn ? 1f : 0f;
        _leaderboardLoadingPanel.interactable = isOn;
        _leaderboardLoadingPanel.blocksRaycasts = isOn;
    }

    //======================================================================================================================================================
    private void OnPersonalEntryLoaded(Dan.Models.Entry entry)
    {
        PlayerLeaderboardEntry.GetComponent<LeaderBoardEntry>().RankText.SetText("" + entry.Rank);
        PlayerLeaderboardEntry.GetComponent<LeaderBoardEntry>().UserNameText.SetText(Username);
        PlayerLeaderboardEntry.GetComponent<LeaderBoardEntry>().ScoreText.SetText("" + entry.Score);
    }
}




