using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public enum GameEventType
{
   GameOver, Enemy_Death, Enemy_Hit, Enemy_Escape, Player_LevelUp, UpgradeBought, ShipSelect,ToggleSlowMo,Pickup
}

public class GameEventSystem
{
    public static Action GameOver;

    public static Action PickupEvent;

    public static Action<string, BaseEnemy> OnEnemyDeath;
    public static Action<string, BaseEnemy> OnEnemyHit;
    public static Action<string, BaseEnemy> OnEnemyEscape;
    public static Action<int,float, float> XpChanged;
    public static Action<UpgradeElement> OnUpgradeBought;
    public static Action<int> OnShipSelect;

    public static Action<bool> ToggleSlowMo;
    
    public static void Call(GameEventType eventType, params object[] args)
    {
        switch (eventType)
        {
            case GameEventType.ToggleSlowMo:
                ToggleSlowMo?.Invoke((bool)args[0]);
                break;
            case GameEventType.GameOver:
                GameOver?.Invoke();
                break;
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
                XpChanged?.Invoke((int)args[0], (float)args[1], (float)args[2]);
                break;
            case GameEventType.UpgradeBought:
                OnUpgradeBought?.Invoke((UpgradeElement)args[0]);
                break;
            case GameEventType.ShipSelect:
                OnShipSelect?.Invoke((int)args[0]);
                break;
            case GameEventType.Pickup:
                PickupEvent?.Invoke();
                break;
            default:
                break;
        }
    }
}
