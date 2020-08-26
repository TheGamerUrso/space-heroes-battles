using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Events

{  /**
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


    public static Action<GameStateEnum, GameStateEnum> OnGameStateChanged;
    public static Action OnLoadDataCompleted;
    public static Action<bool> OnPauseGame;
    public static Action<string, bool> OnSceneLoadStart;
    public static Action<string> OnSceneLoadFinished;
    public static Action<float> OnSceneLoadProgress;



    public static Action PlayerShipHit;
    public static Action PlayerLost;


    public static Func<bool> HyperspaceEnded;


    public static Func<bool> ClaimedReward;
    public static Action<int,RewardTypeEnum> ClaimReward;

    /**
     * BaseGame Mode Events
     */
    public static Action GameEnded;
    public static Action<int, int, int> GameStatsChanged;

    public static Action<string, BossEnemy> BossDied;
    public static Action<string, BossEnemy> BossHit;

    public static Action<string, Enemy> EnemyDied;
    public static Action<string, Enemy> EnemyGotHit;
    public static Action<string, Enemy> EnemyEscaped;


    /**
     * Game Events
     */

    public static Action<int> OnCoinValueChanged;
    public static Action<int> OnScoreValueChanged;
    public static Action OnMultiplierChanged;


    public static Action<float> ShakeCamera;

    /**
     *GameController
     */
    public static Action<GameController> OnGameOver;
    public static Action<GameController> OnWin;




    /**
     * PlayerData Events
     */
    public static Action<int, float, float> OnXpValueChanged;
    public static Action<int> OnLevelValueChanged;
    public static Action OnControlScemeChange;
    public static Action<float> OnSuperUseValueChanged;
    public static Action<int> CoinValueChanged;
    public static Action<float> PowerUpLevelValueChanged;
    public static Action<int> OnPowerPackCollected;
    public static Action<int> OnShipSelectValueChanged;
    public static Action OnModeUnlockedChanged;


    /**
     * Screen Manager
     */
    public static Action<string, bool> OnScreenChanged;

    public static Action<ObjectiveData> OnObjectiveChange;
    public static Action<ObjectivesElement> OnClickEvent = delegate { };


    public class ObjectiveEventArgs : EventArgs
    {
        public ObjectiveData objectiveData { get; set; }

        public ObjectiveEventArgs(ObjectiveData objectiveData)
        {
            this.objectiveData = objectiveData;
        }
    }


    /**
     * Upgrade Events 
     */
    public static Action OnPurchased;

    public static Action RefreshUpdateData;
}

