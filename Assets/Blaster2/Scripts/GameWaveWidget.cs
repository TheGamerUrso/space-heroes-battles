using TMPro;
using UnityEngine;

public class GameWaveWidget : MonoBehaviour
{
    public TextMeshProUGUI WaveText;
    public TextMeshProUGUI EnemiesRemaining;
    public CanvasGroup canvasGroup;
    [SerializeField] protected WaveManager waveManager;


    void Update()
    {
        if (waveManager == null) return;

        canvasGroup.alpha = waveManager.waveData.BossBattleInitiated ? 0 : 1;

        WaveText.text = waveManager.waveData.enemiesSpawnedThisWave + "/" + waveManager.waveData.numberOfEnemiesEachWave;
        EnemiesRemaining.text = "" + waveManager.waveData.Wave;
    }
}
