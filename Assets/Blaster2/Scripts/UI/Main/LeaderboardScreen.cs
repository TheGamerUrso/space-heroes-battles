using UnityEngine;
using System.Collections;
using Dan.Main;

public class LeaderboardScreen : MonoBehaviour
{
    [SerializeField] private CanvasGroup _leaderboardLoadingPanel;

    public GameObject LeaderboardEntry;
    public GameObject PlayerLeaderboardEntry;
    public Transform content;
    public string Username;
    public GameObject errorMessage;

    void OnDestroy()
    {
        if (TheGamerUrso.Leaderboards.Instance != null)
        {
            TheGamerUrso.Leaderboards.Instance.OnLeaderboardValueChanged -= OnLeaderboardLoaded;
            TheGamerUrso.Leaderboards.Instance.OnErrorLoading -= OnErrorLoadingCallback;
               TheGamerUrso.Leaderboards.Instance.OnLoadingLeadeboard -= ToggleLoadingPanel;
        }
    }

    //======================================================================================================================================================
    void Start()
    {
        TheGamerUrso.Leaderboards.Instance.OnLeaderboardValueChanged += OnLeaderboardLoaded;
        TheGamerUrso.Leaderboards.Instance.OnErrorLoading += OnErrorLoadingCallback;
        TheGamerUrso.Leaderboards.Instance.OnLoadingLeadeboard += ToggleLoadingPanel;

        PlayerData playerData = PersistantData.GetPlayerData();
        PlayerLeaderboardEntry.GetComponent<LeaderBoardEntry>().RankText.SetText("-");
        PlayerLeaderboardEntry.GetComponent<LeaderBoardEntry>().UserNameText.SetText(Username);
        PlayerLeaderboardEntry.GetComponent<LeaderBoardEntry>().ScoreText.SetText(playerData.HighScore.ToString());
        TheGamerUrso.Leaderboards.Instance.LoadLeaderboard();
    }

    //======================================================================================================================================================
    private void OnLeaderboardLoaded(Dan.Models.Entry[] entries)
    {
        errorMessage.SetActive(false);
        foreach (Transform t in content)
            Destroy(t.gameObject);

        foreach (var t in entries)
            CreateEntryDisplay(t);


        TheGamerUrso.Leaderboards.Instance.GetPersonalEntry(OnPersonalEntryLoaded);
        ToggleLoadingPanel(false);
    }
    //======================================================================================================================================================
    private void CreateEntryDisplay(Dan.Models.Entry entry)
    {
        GameObject leaderboardGO = Instantiate(LeaderboardEntry, content);
        leaderboardGO.GetComponent<LeaderBoardEntry>().SetEntry(entry);
        if (entry.IsMine())
        {
            PlayerLeaderboardEntry.GetComponent<LeaderBoardEntry>().RankText.SetText(entry.RankSuffix());
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

    private void OnErrorLoadingCallback(string error)
    {
        errorMessage.SetActive(true);
    }
}




