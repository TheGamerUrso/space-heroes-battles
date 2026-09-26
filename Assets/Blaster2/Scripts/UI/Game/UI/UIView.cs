using System;
using UnityEngine;

public class UIView : MonoBehaviour
{
    public bool IsActive=> panel.activeSelf;
    [SerializeField] protected GameObject panel;

    public void Toggle(bool value)
    {
        panel.SetActive(!panel.activeSelf);
    }

    public virtual void Show()
    {
        Toggle(true);
    }

    public virtual void Hide()
    {
        Toggle(false);
    }
}
