using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SurvivalPauseMenuOptionScreen : BaseOptions
{
    public TextMeshProUGUI MissionTitle;

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
