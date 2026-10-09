using TheGamerUrso.Core;
using UnityEditor.MPE;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [SerializeField] private GameObject LevelUpPrefab;
    private IDataService dataService;
    private PlayerData playerData;

    private void Start()
    {
        dataService = GameContext.Get<IDataService>();
        playerData = dataService.GetPlayerData();
        if (playerData == null) return;

        foreach (var playerShipData in playerData.playerShipData)
        {
            playerShipData.OnLevelUp += OnLevelValueChanged;
        }
    }

    private void OnDestroy()
    {
        if (playerData == null) return;
        foreach (var playerShipData in playerData.playerShipData)
        {
            playerShipData.OnLevelUp += OnLevelValueChanged;
        }
    }

    public void OnLevelValueChanged(int Level)
    {
        LevelUpPrefab.SetActive(true);
    }  
}
