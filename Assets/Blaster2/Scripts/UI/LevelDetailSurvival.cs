using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelDetailSurvival : MonoBehaviour
{
    public TextMeshProUGUI score;

    private void OnEnable()
    {
        PlayerData playerData = PersistantData.GetPlayerData();
        score.text = "Highscore \n" + playerData.HighScore;
    }

}
