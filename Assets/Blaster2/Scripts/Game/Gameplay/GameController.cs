using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameController : MonoSingleton<GameController>
{
    public enum GameState
    {
        START, GAME, GAMEOVER, WIN
    }

    public GameState currentGameState = GameState.START;
    public static GameState CurrentGameState
    {
        get
        {
            return Instance.currentGameState;
        }
    }

    [SerializeField] private GameObject EnemyWaypoints;
    private GameObject player;
    private PlayerShip playerShip;
    private PlayerData playerData;
    private PlayerShipData playerShipData;
    private BaseGameMode baseGameMode;
    private float delayTheSlowMoEffectTimer = .3f;
    private float delay = 4;

    public GameObject AsteroidBackgroundSpawner;
    public GameObject PanelBackgroundSpawner;

    public GameObject Tutorial;

    public GameObject GUI;

    private void OnApplicationFocus(bool focus)
    {
#if UNITY_EDITOR

        return;
#endif
        if (Application.platform == RuntimePlatform.Android)
        {
            if (!focus)
            {
                if (Game.IsGameOver == false)
                {
                    GameManager.Instance.PauseTheGame(true);
                }
            }
        }
    }

    private void OnApplicationPause(bool Paused)
    {
#if UNITY_EDITOR
        return;
#endif
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Paused)
            {
                if (Game.IsGameOver == false)
                {
                    GameManager.Instance.PauseTheGame(true);
                }
            }
        }
    }

    protected override void OnCleanup()
    {
        base.OnCleanup();

        Events.PlayerLost -= PlayerLostCallback;
        Events.GameEnded -= Win;
        Events.EnemyDied -= EnemyDied;
    }
    protected override void Awake()
    {
        base.Awake();

        baseGameMode = GameObject.FindObjectOfType<BaseGameMode>();

        if (EnemyWaypoints != null)
            Instantiate(EnemyWaypoints, transform, false);

        if (AsteroidBackgroundSpawner != null)
        {
            Instantiate(AsteroidBackgroundSpawner, transform, false);
        }

        if (PanelBackgroundSpawner != null)
        {
            Instantiate(PanelBackgroundSpawner, transform, false);
        }

        if (Tutorial != null)
        {
            Instantiate(Tutorial, transform, false);
        }
    }

    void Start()
    {
        Application.targetFrameRate = 60;

        playerData = PersistantData.GetPlayerData();
        playerShipData = playerData.GetCurrentPlayerShipData();

        Events.PlayerLost += PlayerLostCallback;
        Events.GameEnded += Win;
        Events.EnemyDied += EnemyDied;

        Game.UseSlowMo = false;

        SetGameState(GameState.START);
    }

    IEnumerator StartGameDelay()
    {
        AudioManager.PlayRandomMusic(true);

        Game.Reset();

        if (PlayerManager.GetPlayer() == null)
        {
            int shipSelected = playerData.CurrrentSelectedShip;
            player = PlayerManager.CreatePlayer(shipSelected);
            playerShip = player.GetComponentInChildren<PlayerShip>();
        }

        baseGameMode.InitReference(playerData, playerShip);

        yield return new WaitForSeconds(2.0f);

        SetGameState(GameState.GAME);
    }

    public void SetGameState(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.START:
                StartCoroutine(StartGameDelay());
                break;
            case GameState.GAME:

                break;
            case GameState.GAMEOVER:
                if (Game.IsGameOver == false)
                {
                    Game.IsGameOver = true;
                    StartCoroutine(DelayGameOver());
                }
                break;
            case GameState.WIN:
                if (!Game.IsGameOver)
                {
                    Game.IsGameOver = true;

                    StartCoroutine(DelayWinScreen());
                }
                break;
        }
        currentGameState = gameState;
    }

    private void EnemyDied(string name, BaseEnemy baseEnemy)
    {
        int PlayerLevel = playerData.GetCurrentPlayerShipData().level;
        int EnemyLevel = baseEnemy.Level;
        int levelDiffrence = PlayerLevel / EnemyLevel;

        if (levelDiffrence == 0)
        {
            levelDiffrence = 1;
        }

        if (!Game.IsSurvivalMode)
        {
            float XPEarned = (2.5f * PlayerLevel) / levelDiffrence;
            playerData.EarnXP(XPEarned);
        }

        playerData.PowerUpLevel += .025f;

        int score = Game.Multiplier * baseEnemy.EnemyData.EnemyValue;
        int kills = Game.EnemyKilled + 1;


        Game.SetCurrentEnemyKills(kills);
        Game.EnemyKilled++;
        Game.Score = score;
        Game.Multiplier++;



        playerData.Kills = kills;
        ObjectiveData objectiveData = playerData.GetOnGoingObjectiveById(ObjectiveType.Kill);

        if (objectiveData != null)
        {
            var newProgress = objectiveData.progress + playerData.Kills;
            objectiveData.UpdateProgress(newProgress);
        }
    }

    private void PlayerLostCallback()
    {
        GameOver();
    }

    public void Win()
    {
        SetGameState(GameState.WIN);
    }

    public void GameOver()
    {
        SetGameState(GameState.GAMEOVER);
    }

    IEnumerator DelayGameOver()
    {
        Time.timeScale = 1.0f;

        playerShipData.Upgrades[(int)UpgradeTypeEnum.Shield] = 0;

        if (Game.IsSurvivalMode)
        {
            GameManager.Instance.SetSurvivalScore(Game.Score);
        }

        SaveSystem.SaveGame();

        yield return new WaitForSeconds(2.0f);

        Events.OnGameOver?.Invoke(this);

        SaveSystem.SaveGame();
    }

    IEnumerator DelayWinScreen()
    {
        Time.timeScale = 1.0f;

        playerShipData.Upgrades[(int)UpgradeTypeEnum.Shield] = 0;

        GameManager.Instance.UpdatePlayerStatistics();
        GameManager.Instance.UnlockNextMission();
        GameManager.Instance.PlayerQuestCheck();

        GameManager.Instance.PostAchievementProgress(GameManager.AchievementType.KILL, playerData.Kills);
       
        SaveSystem.SaveGame();

        yield return new WaitForSeconds(2.0f);

        playerShip?.Exit();

        yield return new WaitForSeconds(2.0f);
        Events.OnWin?.Invoke(this);
 
    }

    private void Update()
    {
        SlowMo();
    }

    public void SlowMo()
    {
        if (currentGameState == GameState.GAME)
        {
            if (delay > 0)
            {
                delay -= Time.deltaTime;
            }
            else
            {
                if (!Game.IsGameOver && !Game.IsPaused && Game.UseSlowMo)
                {
                    if (Game.SlowMo)
                    {
                        Time.timeScale = delayTheSlowMoEffectTimer;
                    }
                    else if (!Game.SlowMo && Time.timeScale < 1)
                    {
                        Time.timeScale = 1.0f;
                    }
                }
            }
        }
        else
        {
            Time.timeScale = 1.0f;
        }
    }
}
