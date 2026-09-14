using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.Core;
using TMPro;
using UnityEngine;

public class LevelWidget : MonoBehaviour
{
    private PlayerData playerData;
    private PlayerShipData playerShipData;
    [SerializeField] private TextMeshProUGUI PlayerLevelText;

    private bool updateText;
    private int level;
    private float ActualLevelToShow;
    private IDataService dataService;

    private void Awake()
    {
        dataService = GameContext.Get<IDataService>();
    }


    private void Start()
    {
       // Events.OnLevelValueChanged += SetPlayerLevelText;
        Events.OnShipSelectValueChanged += NewShipSelected;

        playerData = dataService.GetPlayerData();
        playerShipData = playerData.GetCurrentPlayerShipData();

        SetPlayerLevelText(playerShipData.level);
    }

    private void OnDestroy()
    {
        //Events.OnLevelValueChanged -= SetPlayerLevelText;
        Events.OnShipSelectValueChanged -= NewShipSelected;
    }

    private void Update()
    {
        if (updateText)
        {
            ActualLevelToShow = Mathf.Lerp(ActualLevelToShow, level, .4f);
            PlayerLevelText.text = string.Format("{0}", Mathf.Round(ActualLevelToShow));
            if (ActualLevelToShow == level)
            {
                updateText = false;
            }
        }
    }

    public void NewShipSelected(int shipSelected)
    {
        playerShipData = playerData.GetCurrentPlayerShipData();

        SetPlayerLevelText(playerShipData.level);
    }

    public void SetPlayerLevelText(int lvl)
    {
        level = lvl;
        updateText = true;
    }
}
