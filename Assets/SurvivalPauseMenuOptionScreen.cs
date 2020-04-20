using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SurvivalPauseMenuOptionScreen : BaseOptions
{
    public TextMeshProUGUI MissionTitle;
    private PlayerData playerData;

    public override void InitializeOptions()
    {
        base.InitializeOptions();
        playerData = PersistantData.GetPlayerData();
    }

    public override void OnOptionEnter()
    {
        if (GameManager.Instance != null)
            base.OnOptionEnter();
    }

    public override void ExitAndSave()
    {
        base.ExitAndSave();
        GuiManager.Instance.ResumeButton();
    }

    public void Quit()
    {
        base.ExitAndSave();
        GuiManager.Instance.LoadMainMenu();
    }
}
