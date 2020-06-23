using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Events
{
    public static Action<EGameState, EGameState> OnGameStateChanged;
    public static Action OnLoadDataCompleted;
    public static Action<bool> OnPauseGame;
    public static Action<string, bool> OnSceneLoadStart;
    public static Action<string, bool> OnSceneLoadFinished;
    public static Action<float> OnSceneLoadProgress;



    public static Action PlayerShipHit;
    public static Action PlayerShipDeath;


    public static Func<bool> OnWaveEnded;


    /**
     * BaseGame Mode Events
     */
    public static Action SpawnEnded;
    public static Action<int, int, int> GameStatsChanged;
    public static Action<string, BaseEnemy> BossDied;

    public static Action<string, BaseEnemy> EnemyDied;
    public static Action<string, BaseEnemy> EnemyGotHit;
    public static Action<string, BaseEnemy> EnemyEscaped;


    /**
     * Game Events
     */

    public static Action<int> OnCoinValueChanged;
    public static Action<int> OnScoreValueChanged;
    public static Action<int> OnMultiplierChanged;



    /**
     *GameController
     */
    public static Action<GameController> OnGameOver;
    public static Action<GameController> OnWin;




    /**
     * PlayerData Events
     */
    public delegate void XpValueChanged(int level, float xp, float xpToLevel);
    public delegate void LevelValueChanged(int level);
    public delegate void DistanceChanged(float ammount);
    public delegate void SuperUseValueChanged(float ammount);
    public delegate void CoinValueChanged(int ammount);
    public delegate void PowerUpLevelChanged(float ammount);
    public delegate void PowerPackCollected(int ammount);
    public delegate void ShipSelectValueChanged(int selection);
    public delegate void ModeUnlocked();


    /**
     * Screen Manager
     */
    public static Action<string, bool> OnScreenChanged;

    public static EventHandler<ObjectiveEventArgs> OnObjectiveChange;
    public static Action<ObjectivesElement> OnClickEvent = delegate { };


    public class ObjectiveEventArgs : EventArgs
    {
        public ObjectiveData objectiveData { get; set; }

        public ObjectiveEventArgs(ObjectiveData objectiveData)
        {
            this.objectiveData = objectiveData;
        }
    }
}

