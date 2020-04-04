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
        Popup.Show(Popup.popupType.error, "Settings Saved", true);
        StartCoroutine(SaveAndExitCoroutine());
    }

    private IEnumerator SaveAndExitCoroutine()
    {

        yield return new WaitForSeconds(1);
        ScreenManager.Instance.Close();
    }


}