using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuOptionScreen : BaseOptions
{
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
