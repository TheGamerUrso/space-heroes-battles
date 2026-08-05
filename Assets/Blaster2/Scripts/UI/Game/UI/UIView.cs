using System;
using UnityEngine;

public class UIView : MonoBehaviour
{
    public bool IsVisible { get; private set; }
    public bool IsActive() {  return IsVisible; }
    public virtual void Toggle(bool value)
    {
       
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
