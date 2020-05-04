using TMPro;
using UnityEngine;

public class ResizeContent : MonoBehaviour
{
    public TextMeshProUGUI TextMeshProUGUI;

    // Use this for initialization
    void Start()
    {
        TextMeshProUGUI = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update()
    {
        if (GetComponent<RectTransform>().sizeDelta.y != TextMeshProUGUI.textBounds.size.y)
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(0, TextMeshProUGUI.textBounds.size.y);
        }

    }
}
