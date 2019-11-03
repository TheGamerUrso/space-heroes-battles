using System;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public class MouseInput
{
    private const String HorizontalMouse = "HorizontalMouse";
    private const int MOUSE = 0;
    private Touch currentTouch;
    private TouchPhase currentTouchPhase;
    private Vector2 previousTouchPos;

    private bool blockMovement;

    public Vector3 GetMousePos()
    {
        return Input.mousePosition;
    }
    public bool CheckIfTouchIsOverUI(Touch touch)
    {
        int id = touch.fingerId;
        if (EventSystem.current.IsPointerOverGameObject(id))
        {
            return true;
        }

        return false;
    }

    public Vector2 GetTouchPosition()
    {
        if (Input.touchCount > 0)
        {
            currentTouch = Input.GetTouch(0);
            currentTouchPhase = currentTouch.phase;
            blockMovement = CheckIfTouchIsOverUI(currentTouch);

            if (!blockMovement)
            {
                switch (currentTouchPhase)
                {
                    case TouchPhase.Began:
                        previousTouchPos = currentTouch.position;
                        return currentTouch.position;
                    case TouchPhase.Moved:
                        previousTouchPos = currentTouch.position;
                        return currentTouch.position;
                    case TouchPhase.Stationary:
                        previousTouchPos = currentTouch.position;
                        return currentTouch.position;
                    case TouchPhase.Ended:
                        return previousTouchPos;
                    case TouchPhase.Canceled:
                        return previousTouchPos;
                    default:
                        return previousTouchPos;
                }
            }
        }
        return previousTouchPos;
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