using TMPro;
using UnityEngine;

public class HighscoreScreen : MonoBehaviour
{
    private PlayerData playerData;
    public TextMeshProUGUI HighScore;

    private void Start()
    {
        UpdateScore();
    }

    public void UpdateScore()
    {
        PlayerData playerData = PersistantData.GetPlayerData();


            float highscore = playerData.GetHighScore();
            string scoreText = string.Format("{00:00000000}", highscore);
            HighScore.text = string.Format("{0}", scoreText);
        

    }
}
