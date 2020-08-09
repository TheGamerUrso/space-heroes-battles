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

    public bool updateText;

    private float xp;
    private float xpToLevel;

    private float ActualXpToShow;
    private float ActualXpToLevelToShow;

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

    private void Update()
    {
        if (updateText) {
            ActualXpToShow = Mathf.Lerp(ActualXpToShow, xp, .2f);
            ActualXpToLevelToShow = Mathf.Lerp(ActualXpToLevelToShow, xpToLevel, .2f);
            PlayerXPText.text = Mathf.Round(xp) + " / " + Math.Round(xpToLevel);
            if (ActualXpToLevelToShow == xpToLevel && ActualXpToShow == xp)
            {
                updateText = false;
            }
        }
    }
    public void SetPlayerXP(int lvl, float xp, float xpToLevel)
    {
        this.xp = xp;
        this.xpToLevel = xpToLevel;
        updateText = true;
    }
}
