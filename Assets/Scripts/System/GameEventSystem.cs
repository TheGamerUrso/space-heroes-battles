using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum GameEventType
{
    Enemy_Death, Enemy_Hit, Enemy_Escape, Player_LevelUp, UpgradeBought, ShipSelect
}

public class GameEventSystem
{
    public static Action<string, BaseEnemy> OnEnemyDeath;
    public static Action<string, object> OnEnemyHit;
    public static Action<string, BaseEnemy> OnEnemyEscape;
    public static Action OnPlayerLevelUp;
    public static Action<UpgradeElement> OnUpgradeBought;
    public static Action<int> OnShipSelect;

    public static void Call(GameEventType eventType, params object[] args)
    {
        switch (eventType)
        {
            case GameEventType.Enemy_Death:
                OnEnemyDeath?.Invoke((string)args[0], (BaseEnemy)args[1]);
                break;
            case GameEventType.Enemy_Hit:
                OnEnemyHit?.Invoke((string)args[0], (BaseEnemy)args[1]);
                break;
            case GameEventType.Enemy_Escape:
                OnEnemyEscape?.Invoke((string)args[0], (BaseEnemy)args[1]);
                break;
            case GameEventType.Player_LevelUp:
                OnPlayerLevelUp?.Invoke();
                break;
            case GameEventType.UpgradeBought:
                OnUpgradeBought?.Invoke((UpgradeElement)args[0]);
                break;
            case GameEventType.ShipSelect:
                OnShipSelect?.Invoke((int)args[0]);
                break;
            default:
                break;
        }
    }
}
