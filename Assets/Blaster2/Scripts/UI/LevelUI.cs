using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.Core;
using TMPro;
using UnityEngine;

public class LevelUI : MonoBehaviour
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
        playerData = dataService.GetPlayerData();
        playerShipData = playerData.GetCurrentPlayerShipData();
        playerData.OnCurrencyValueChanged += PlayerData_OnCurrentShipSelectedValueChanged;
        playerData.OnCurrentShipSelectedValueChanged += PlayerData_OnCurrentShipSelectedValueChanged;
        playerData.GetCurrentPlayerShipData().OnLevelUp += PlayerShipData_OnLevelUp;

        SetText(playerShipData.Level);
    }
    private void OnDestroy()
    {
        playerData.OnCurrencyValueChanged -= PlayerData_OnCurrentShipSelectedValueChanged;
        playerData.OnCurrentShipSelectedValueChanged -= PlayerData_OnCurrentShipSelectedValueChanged;
        playerData.GetCurrentPlayerShipData().OnLevelUp -= PlayerShipData_OnLevelUp;
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

    public void PlayerData_OnCurrentShipSelectedValueChanged(int shipSelected)
    {
        playerShipData = playerData.GetCurrentPlayerShipData();
        SetText(playerShipData.Level);
    }

    public void PlayerShipData_OnLevelUp(int lvl)
    {
        SetText(lvl);
    }

    public void SetText(int lvl)
    {
        level = lvl;
        updateText = true;
    }
}
