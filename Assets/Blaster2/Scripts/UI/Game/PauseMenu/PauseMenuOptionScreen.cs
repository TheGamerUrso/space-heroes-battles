using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuOptionScreen : BaseOptions
{
    public override void ExitAndSave()
    {
        base.ExitAndSave();
       // GuiManager.Instance.ResumeButton();
    }

    public void Quit()
    {
        base.ExitAndSave();
        Time.fixedDeltaTime = 0.02f;
       // GuiManager.Instance.LoadMainMenu();
    }
}
