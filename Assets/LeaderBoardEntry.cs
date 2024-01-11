using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Leaderboards;

public class LeaderBoardEntry : MonoBehaviour
{
    public TextMeshProUGUI RankText;
    public TextMeshProUGUI UserNameText;
    public TextMeshProUGUI ScoreText;

    public void SetEntry(Entry entry)
    {
        RankText.text = ""+entry.Rank;
        UserNameText.text = entry.Username;
        ScoreText.text = string.Format("{00:00000000}", entry.Score.ToString());
        GetComponent<Image>().color = entry.IsMine() ? Color.yellow : Color.white;
    }
}
