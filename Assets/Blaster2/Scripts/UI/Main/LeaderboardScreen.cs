using UnityEngine;
using System.Collections;
using static Leaderboards;

public class LeaderboardScreen : MonoBehaviour
{
    public GameObject LeaderboardEntry;
    public GameObject PlayerLeaderboardEntry;
    public Transform content;

    void Start()
    {
        PlayerData playerData = PersistantData.GetPlayerData();
        PlayerLeaderboardEntry.GetComponent<LeaderBoardEntry>().RankText.SetText("-");
        PlayerLeaderboardEntry.GetComponent<LeaderBoardEntry>().UserNameText.SetText(playerData.Username);
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

