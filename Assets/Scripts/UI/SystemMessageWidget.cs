using System.Collections;
using TMPro;
using UnityEngine;

public class SystemMessageWidget : MonoBehaviour
{
    public TextMeshProUGUI WidgetText;
    public Doozy.Engine.UI.UIView uiView;

    public void Enabled(bool value = true)
    {
        uiView.Toggle(value);
    }

    public void AutoClose()
    {
        StartCoroutine(Close());
    }

    IEnumerator Close()
    {
        yield return new WaitForSeconds(1.0f);
        Enabled(false);
    }

    public void SetWidgetText(string score)
    {
        WidgetText.text = score;
    }
}
