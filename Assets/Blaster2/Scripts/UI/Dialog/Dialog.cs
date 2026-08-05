using System.Collections;
using TMPro;
using UnityEngine;

public class Dialog<T> : MonoBehaviour
{
    public TextMeshProUGUI WidgetText;
    public UIView uiView;

    public void Enabled(bool value = true)
    {
        uiView.Toggle(value);
    }
    public virtual void OK()
    {
        Close();
    }
    public void Close()
    {
        Enabled(false);
    }

    public void AutoClose()
    {
        StartCoroutine(CloseWithDelay());
    }

    IEnumerator CloseWithDelay()
    {
        yield return new WaitForSeconds(1.0f);
        Enabled(false);
    }

    public void SetWidgetText(string score)
    {
        WidgetText.text = score;
    }
}
