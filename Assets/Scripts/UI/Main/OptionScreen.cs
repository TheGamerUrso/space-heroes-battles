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
        ScreenManager.Instance.Close();

        Popup.Show(Popup.popupType.error, "Settings Saved", false);
    }

}