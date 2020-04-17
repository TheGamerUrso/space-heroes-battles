using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public enum BossEventType {
    BossDeath, BossHit,BossChangePhase
}

public enum EnemyEventType
{
    Enemy_Death, Enemy_Hit, Enemy_Escape
}

public enum PlayerEventType
{
    Player_LevelUp, Pickup
}

public enum GameEventType
{
   GameOver, UpgradeBought, ShipSelect,ToggleSlowMo,PauseGame
}

public class GameEventSystem
{
    /**
     * Player Events
     */
    public static Action PlayerLeveledUp;
    public static Action PickUpEvent;
    public static Action<int, float, float> XpChanged;
    public static Action<float> PowerUpChanged;

    /**
     * Game Events
     */
    public static Action GameOver;
    public static Action<int> OnShipSelect;
    public static Action<UpgradeElement> OnUpgradeBought;
    public static Action<bool> ToggleSlowMo;
    public static Action<bool> OnPauseGame;

    /**
     * Enemy Events
     */
    public static Action<string, BaseEnemy> OnEnemyDeath;
    public static Action<string, BaseEnemy> OnEnemyHit;
    public static Action<string, BaseEnemy> OnEnemyEscape;

    /**
     * Boss Events
     */
    public static Action<string,BaseBossEnemy,float> BossHitEvent;
    public static Action<string, BaseBossEnemy> BossDied;
    public static Action<string, BaseBossEnemy,int> BossPhaseChanged;

    public static void Call(BossEventType eventType, params object[] args)
    {
        switch (eventType)
        {
            case BossEventType.BossDeath:
                BossDied?.Invoke((string)args[0], (BaseBossEnemy)args[1]);
                break;
            case BossEventType.BossHit:
                BossHitEvent?.Invoke((string)args[0],(BaseBossEnemy)args[1],(float)args[2]);
                break;
            case BossEventType.BossChangePhase:
                BossPhaseChanged?.Invoke(
                    (string)args[0], (BaseBossEnemy)args[1], (int)args[2]);
                break;
        }
    }

    public static void Call(PlayerEventType eventType, params object[] args)
    {
        switch (eventType)
        {
            case PlayerEventType.Pickup:
                PickUpEvent?.Invoke();
                break;
            case PlayerEventType.Player_LevelUp:
                PlayerLeveledUp?.Invoke();
                break;
        }
    }
    public static void Call(EnemyEventType eventType, params object[] args)
    {
        switch (eventType)
        {
            case EnemyEventType.Enemy_Death:
                OnEnemyDeath?.Invoke((string)args[0], (BaseEnemy)args[1]);
                break;
            case EnemyEventType.Enemy_Hit:
                OnEnemyHit?.Invoke((string)args[0], (BaseEnemy)args[1]);
                break;
            case EnemyEventType.Enemy_Escape:
                OnEnemyEscape?.Invoke((string)args[0], (BaseEnemy)args[1]);
                break;
        }
    }

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

            case GameEventType.UpgradeBought:
                OnUpgradeBought?.Invoke((UpgradeElement)args[0]);
                break;
            case GameEventType.ShipSelect:
                OnShipSelect?.Invoke((int)args[0]);
                break;
            case GameEventType.PauseGame:
                OnPauseGame?.Invoke((bool)args[0]);
                break;
            default:
                break;
        }
    }
}
