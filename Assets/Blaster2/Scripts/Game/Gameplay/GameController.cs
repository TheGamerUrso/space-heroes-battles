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
        playerData.SetSuperMeter(0);
        playerData.ResetWeaponPowerUPCollected();
        Game.Reset();

        yield return new WaitForSeconds(1.0f);

        if (PlayerManager.GetPlayer() == null)
        {
            int shipSelected = playerData.CurrrentSelectedShip;
            player = PlayerManager.CreatePlayer(shipSelected);
            playerShip = player.GetComponentInChildren<PlayerShip>();
        }

        baseGameMode.InitReference(playerData, playerShip);
        playerShip.DisableFire();
        yield return new WaitForSeconds(2.0f);
        playerShip.EnableFire();
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
        if (baseEnemy.Id.Equals(name))
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

                GuiManager.CreateFloatingText("<color=" + "yellow" + ">" + XPEarned + "</color>" + "<color=" + "orange" + "> XP </color>", baseEnemy.transform.localPosition);
            }

            playerData.SetSuperMeter(playerData.PowerUpLevel + 0.025f);

            int kills = Game.EnemyKilled + 1;
            int score = baseEnemy.EnemyData.EnemyValue;


            if (Game.Multiplier > 0)
            {
                score = Game.Multiplier * baseEnemy.EnemyData.EnemyValue;
                GuiManager.SetScoreMultipler(score + "(x" + Game.Multiplier + ")");
            }
            else
            {
                GuiManager.SetScoreMultipler("" + score);
            }

            Game.SetCurrentEnemyKills(kills);

            Game.SetScore(score);

            Game.IncreaseMultiplier();

            playerData.Kills = kills;

            GameManager.Instance.PlayerQuestProgress(ObjectiveTypeEnum.SCORE, score);
            GameManager.Instance.PlayerQuestProgress(ObjectiveTypeEnum.KILL, 1);
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
        GameManager.Instance.PlayerChallengesCheck();
        GameManager.Instance.UpdatePlayerStatistics();

        SaveSystem.SaveGame();

        yield return new WaitForSeconds(2.0f);

        playerShip?.Exit();

        yield return new WaitForSeconds(2.0f);
        Events.OnWin?.Invoke(this);

    }

   
}
