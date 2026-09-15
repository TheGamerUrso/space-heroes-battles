using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.Core;
using TMPro;
using UnityEngine;

public class GameWaveWidget : MonoBehaviour
{
    public TextMeshProUGUI WaveText;
    public TextMeshProUGUI EnemiesRemaining;
    public CanvasGroup canvasGroup;
    [SerializeField] protected GameMode gameMode;


    void Update()
    {
        if (gameMode == null) return;

        canvasGroup.alpha = gameMode.BossBattleInitiated ? 0 : 1;

        WaveText.text = gameMode.CurrentTotalEnemies + "/" + gameMode.TotalEnemies;
        EnemiesRemaining.text = "" + gameMode.waves;
    }
}
    