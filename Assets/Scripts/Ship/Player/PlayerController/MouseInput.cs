using System;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public class MouseInput
{
    private const String HorizontalMouse = "HorizontalMouse";
    private const int MOUSE = 0;

    public Vector3 GetMousePos()
    {
        return Input.mousePosition;
    }

    public bool GetClickDown()
    {
        if (Input.GetMouseButton(MOUSE))
        {
            return true;
        }

        return false;
    }

    public float GetMouseVelocity()
    {
        return Input.GetAxis(HorizontalMouse);
    }
}