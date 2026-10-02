using System;
using UnityEngine;

public class UIView : MonoBehaviour
{
    public bool IsActive=> panel.activeSelf;
    [SerializeField] protected GameObject panel;

    public virtual void Show()
    {
        panel.SetActive(true);
    }

    public virtual void Hide()
    {
        panel.SetActive(false);
    }
}
