using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelDetailSurvival : MonoBehaviour
{
    public TextMeshProUGUI score;

    public void Update()
    {
        PlayerData playerData = PersistantData.GetPlayerData();
        score.text = "Highscore \n" + playerData.GetHighScore(0);
    }
}
