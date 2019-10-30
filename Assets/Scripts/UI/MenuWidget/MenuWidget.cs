using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuWidget : MonoBehaviour
{
    public RectTransform MenuWindow;
    private Vector2 startingSize;

    private void Start()
    {
        startingSize = new Vector2(0,150);
    }
    public void OpenMenu()
    {
        MenuWindow.gameObject.SetActive(true);
    }

    public void CloseMenu()
    {
        MenuWindow.gameObject.SetActive(false);
    }
}
