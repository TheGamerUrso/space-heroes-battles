using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SurvivalHighscore : MonoBehaviour
{
    public TextMeshProUGUI highscoreText;
    public TextMeshProUGUI scoreText;

    private void OnEnable()
    {
        PlayerData playerData = DataController.GetPlayerData();
        float score =  playerData.GetScore(0);
        float highscore = playerData.GetHighScore(0);
        highscoreText.text = string.Format("{00:00000000}", highscore);
        scoreText.text = string.Format("{00:00000000}", score);
    }
}
