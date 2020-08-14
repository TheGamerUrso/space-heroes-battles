using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class XPWidget : MonoBehaviour
{
    private PlayerData playerData;
    private PlayerShipData playerShipData;
    [SerializeField] private TextMeshProUGUI PlayerXPText;

    private void OnDestroy()
    {
        Events.OnXpValueChanged -= SetPlayerXP;
        Events.OnShipSelectValueChanged -= NewShipSelected;
    }

    private void Start()
    {
        Events.OnXpValueChanged += SetPlayerXP;
        Events.OnShipSelectValueChanged += NewShipSelected;

        playerData = PersistantData.GetPlayerData();
        playerShipData = playerData.GetCurrentPlayerShipData();

        SetPlayerXP(playerShipData.level, playerShipData.xp, playerShipData.xpToLevel);
    }

    public void NewShipSelected(int shipSelected)
    {
        playerShipData = playerData.GetCurrentPlayerShipData();

        SetPlayerXP(playerShipData.level, playerShipData.xp, playerShipData.xpToLevel);
    }

    public void SetPlayerXP(int lvl, float xp, float xpToLevel)
    {
        PlayerXPText.text = Mathf.Round(xp) + " / " + Math.Round(xpToLevel);
    }
}
