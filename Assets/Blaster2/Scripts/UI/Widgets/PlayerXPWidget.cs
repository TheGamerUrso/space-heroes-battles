using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerXPWidget : MonoBehaviour
{
    [SerializeField] private PlayerShip player;
    private PlayerData playerData;
    private PlayerShipData playerShipData;


    [SerializeField] private Slider XPBar = null;
    [SerializeField] private TextMeshProUGUI XPStatus;
    private IDataService dataService;

    private void Awake()
    {
        dataService = GameContext.Get<IDataService>();
    }

    private void Start()
    {
        playerData = dataService.GetPlayerData();
        playerShipData = playerData.GetCurrentPlayerShipData();

        UpdateXP(playerShipData.level, playerShipData.xp, playerShipData.xpToLevel);
    }

    public void UpdateXP(int lvl, float xp, float xpToLevel)
    {
        XPBar.maxValue = xpToLevel;
        XPBar.value = xp;
        XPStatus.text = Mathf.Round(xp) + "/" + Mathf.Round(xpToLevel);
    }
}
