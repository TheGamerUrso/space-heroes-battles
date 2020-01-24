using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum EventType {
    Enemy_Death,Enemy_Hit,Enemy_Escape,Player_LevelUp
}

public class GameEventSystem
{
    public delegate void OnEnemyDeath(BaseEnemy baseEnemy);
    public static OnEnemyDeath OnEnemyDeathHandled;

    public delegate void OnEnemyHit(object hit);
    public static OnEnemyHit OnEnemyHithHandled;

    public delegate void OnEnemyEscape(BaseEnemy baseEnemy);
    public static OnEnemyEscape OnEnemyEscapeHandled;


    public delegate void OnPlayerLevelUp();
    public static OnPlayerLevelUp OnPlayerLevelUpHandled;


    public static void Call(EventType eventType , params object[] args)
    {
        switch (eventType)
        {
            case EventType.Enemy_Death:
                OnEnemyDeathHandled?.Invoke((BaseEnemy)args[0]);
                break;
            case EventType.Enemy_Hit:
                OnEnemyHithHandled?.Invoke(args[0]);
                break;
            case EventType.Enemy_Escape:
                OnEnemyEscapeHandled?.Invoke((BaseEnemy)args[0]);
                break;
            case EventType.Player_LevelUp:
                OnPlayerLevelUpHandled?.Invoke();
                break;
            default:
                break;
        }
    }
}
