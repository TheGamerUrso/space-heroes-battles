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
    [SerializeField] protected GameController gameController;


    void Update()
    {
        if (gameController == null) return;

        canvasGroup.alpha = gameController.BossBattleInitiated ? 0 : 1;

        WaveText.text = gameController.CurrentTotalEnemies + "/" + gameController.TotalEnemies;
        EnemiesRemaining.text = "" + gameController.waves;
    }
}
    