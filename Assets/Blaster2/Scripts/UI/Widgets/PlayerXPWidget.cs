using System.Collections;
using System.Collections.Generic;
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

    private void OnDestroy()
    {
        if (player != null)
        {
            Events.OnXpValueChanged -= UpdateXP;
        }
    }

    private void Start()
    {
        playerData = PersistantData.GetPlayerData();
        playerShipData = playerData.GetCurrentPlayerShipData();

        Events.OnXpValueChanged += UpdateXP;

        UpdateXP(playerShipData.level, playerShipData.xp, playerShipData.xpToLevel);

    }

    public void UpdateXP(int lvl, float xp, float xpToLevel)
    {
        XPBar.maxValue = xpToLevel;
        XPBar.value = xp;
        XPStatus.text = Mathf.Round(xp) + "/" + Mathf.Round(xpToLevel);
    }
}
