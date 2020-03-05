using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
public enum GameEventType
{
    UIPanelToggle
}

public static class GameUIEvents
{
 

    public static Action<bool> ShowLevelDetails;

    public static void Call(GameEventType gameEventType,params object[] parameters)
    {
        switch (gameEventType)
        {
            case GameEventType.UIPanelToggle:
                ShowLevelDetails.Invoke((bool)parameters[0]);
                break;
            default:
                break;
        }      
    }
}
