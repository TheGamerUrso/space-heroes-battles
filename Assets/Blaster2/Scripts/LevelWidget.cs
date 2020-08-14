using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelWidget : MonoBehaviour
{
    private PlayerData playerData;
    private PlayerShipData playerShipData;
    [SerializeField] private TextMeshProUGUI PlayerLevelText;

    private void OnDestroy()
    {
        Events.OnLevelValueChanged -= SetPlayerLevelText;
        Events.OnShipSelectValueChanged -= NewShipSelected;
    }

    private void Start()
    {
        Events.OnLevelValueChanged += SetPlayerLevelText;
        Events.OnShipSelectValueChanged += NewShipSelected;

        playerData = PersistantData.GetPlayerData();
        playerShipData = playerData.GetCurrentPlayerShipData();

        SetPlayerLevelText(playerShipData.level);
    }
    public void NewShipSelected(int shipSelected)
    {
        playerShipData = playerData.GetCurrentPlayerShipData();

        SetPlayerLevelText(playerShipData.level);
    }

    public void SetPlayerLevelText(int lvl)
    {
        PlayerLevelText.text = string.Format("{0}", lvl);

    }
}
