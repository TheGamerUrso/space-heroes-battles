using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameWaveWidget : MonoBehaviour
{
    public TextMeshProUGUI WaveText;
    public SurvivalMode survivalMode;

    void Update()
    {
        WaveText.text = "" + survivalMode.gameInfo.CurrentTotalEnemies + " / " + survivalMode.gameInfo.TotalEnemies;
    }
}
