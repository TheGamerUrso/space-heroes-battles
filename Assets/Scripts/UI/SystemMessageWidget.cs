using TMPro;
using UnityEngine;

public class SystemMessageWidget : MonoBehaviour
{
    public TextMeshProUGUI WidgetText;
    public GameObject m_Canvas;

    public void Enabled(bool value = true)
    {
        m_Canvas.SetActive(value);
    }



    public void SetWidgetText(string score)
    {
        WidgetText.text = score;
    }
}
