using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionScreen : GooglePlayOptions
{
    public override void ExitAndSave()
    {
        base.ExitAndSave();
        MainMenuManager.Instance.Close();
    }

}