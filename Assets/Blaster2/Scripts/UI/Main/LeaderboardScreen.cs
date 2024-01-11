using TMPro;
using UnityEngine;
using Dan.Main;
using Dan.Models;
using System.Linq;
using Dan.Enums;
using UnityEngine.Diagnostics;
using System.Collections;

public class LeaderboardScreen : MonoBehaviour
{

    public struct LeaderboardSearchQuery
    {
        public int Skip { get; set; } //amount of entries to skip
        public int Take { get; set; } //amount of entries to take
        public string Username { get; set; }
        public TimePeriodType TimePeriod { get; set; }
    }
    public GameObject LeaderboardEntry;
    public GameObject PlayerLeaderboardEntry;
    public Transform content;

    void Start()
    {
        PlayerData playerData = PersistantData.GetPlayerData();
        PlayerLeaderboardEntry.GetComponent<LeaderBoardEntry>().RankText.SetText("-");
        PlayerLeaderboardEntry.GetComponent<LeaderBoardEntry>().UserNameText.SetText(Leaderboards.Instance.Username);
        PlayerLeaderboardEntry.GetComponent<LeaderBoardEntry>().ScoreText.SetText(playerData.HighScore.ToString());
        UpdateScore();
    }

    IEnumerator UpdateLeaderboard()
    {
        foreach (Transform t in content)
            Destroy(t.gameObject);

        yield return new WaitForSeconds(1.0f);

        foreach (Entry entry in Leaderboards.Instance.GetEntries())
        {
            if (entry.IsMine())
            {
                PlayerLeaderboardEntry.GetComponent<LeaderBoardEntry>().SetEntry(entry);
            }

            GameObject leaderboardGO = Instantiate(LeaderboardEntry, content);
            leaderboardGO.GetComponent<LeaderBoardEntry>().SetEntry(entry);
            yield return new WaitForSeconds(.1f);
        }
    }
    public void UpdateScore()
    {
        StartCoroutine(UpdateLeaderboard());
    }
}


public static class GameObjectExtensions
{
    public static void DestroyAllChildren(this GameObject go)
    {
        foreach (Transform transform in go.transform)
        {
            UnityEngine.Object.Destroy(transform.gameObject);
        }
    }
}

