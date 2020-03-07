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
            public delegate void OnEnemyDeath(BaseEnemy baseEnemy);
            public static OnEnemyDeath OnEnemyDeathHandled;

            public delegate void OnEnemyHit(object hit);
            public static OnEnemyHit OnEnemyHithHandled;

            public delegate void OnEnemyEscape(BaseEnemy baseEnemy);
            public static OnEnemyEscape OnEnemyEscapeHandled;


            public delegate void OnPlayerLevelUp();
            public static OnPlayerLevelUp OnPlayerLevelUpHandled;

            public delegate void OnUpgradeBought(UpgradeElement upgradeElement);
            public static OnUpgradeBought OnUpgradeBoughtHandled;

            public delegate void OnShipSelect(int shipSelected);
            public static OnShipSelect OnShipSelectHandled;

            public static void Call(GameEventType eventType, params object[] args)
            {
                switch (eventType)
                {
                    case GameEventType.Enemy_Death:
                        OnEnemyDeathHandled?.Invoke((BaseEnemy)args[0]);
                        break;
                    case GameEventType.Enemy_Hit:
                        OnEnemyHithHandled?.Invoke(args[0]);
                        break;
                    case GameEventType.Enemy_Escape:
                        OnEnemyEscapeHandled?.Invoke((BaseEnemy)args[0]);
                        break;
                    case GameEventType.Player_LevelUp:
                        OnPlayerLevelUpHandled?.Invoke();
                        break;
                    case GameEventType.UpgradeBought:
                        OnUpgradeBoughtHandled?.Invoke((UpgradeElement)args[0]);
                        break;
                    case GameEventType.ShipSelect:
                        OnShipSelectHandled?.Invoke((int)args[0]);
                        break;
                    default:
                        break;
                }
            }
        }
