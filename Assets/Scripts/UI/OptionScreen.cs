using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionScreen : GooglePlayOptions
{
    private MainMenuManager mainMenuManager;

    public override void OnOptionEnter()
    {
        if (mainMenuManager == null)
            mainMenuManager = MainMenuManager.instance;
    }

    public override void ExitAndSave()
    {
        base.ExitAndSave();
        if (mainMenuManager)
        {
            mainMenuManager.ShowMessage("Settings Saved");
        }
    }



}