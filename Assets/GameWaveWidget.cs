using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameWaveWidget : MonoBehaviour
{
    public TextMeshProUGUI WaveText;
    public TextMeshProUGUI EnemiesRemaining;
    public BaseGameMode survivalMode;
    public CanvasGroup canvasGroup;
    void Start()
    {
        survivalMode = GameController.GetGameMode();
    }
    void Update()
    {
        if (survivalMode == null) return;

        canvasGroup.alpha = survivalMode.gameInfo.BossBattleInitiated ? 0 : 1;

        WaveText.text = survivalMode.gameInfo.CurrentTotalEnemies + "/" + survivalMode.gameInfo.TotalEnemies;
        EnemiesRemaining.text = "" + survivalMode.gameInfo.waves;
    }
}
