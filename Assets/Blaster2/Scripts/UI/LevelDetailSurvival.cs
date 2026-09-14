using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.Core;
using TMPro;
using UnityEngine;

public class LevelDetailSurvival : MonoBehaviour
{
    public TextMeshProUGUI score;
    private IDataService dataService;



    private void Awake()
    {
        dataService = GameContext.Get<IDataService>();

        score.text = "Highscore \n" + dataService.GetPlayerData().HighScore;
    }

}
