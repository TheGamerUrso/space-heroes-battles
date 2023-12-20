using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;



public class GameController : MonoSingleton<GameController>
{
    public enum GameState
    {
        START, GAME, GAMEOVER, WIN
    }

    private GameState currentGameState = GameState.START;

    public static GameState CurrentGameState
    {
        get
        {
            if (Instance == null)
            {
                return GameState.GAME;
            }
            return Instance.currentGameState;
        }
    }

    [SerializeField] private GameObject EnemyWaypoints;
    private GameObject player;
    private PlayerShip playerShip;
    private PlayerData playerData;
    private PlayerShipData playerShipData;
    private BaseGameMode baseGameMode;

    public bool HasAsteroids;
    public GameObject AsteroidBackgroundSpawner;
    public GameObject Tutorial;

    public GameObject GUI;

    private void OnApplicationFocus(bool focus)
    {
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

        Events.PlayerLost -= GameOver;
        Events.GameEnded -= Win;
        Events.EnemyDied -= EnemyDied;
        Events.EnemyGotHit -= EnemyGotHitCallback;
        Events.BossHit -= BossEnemyHitCallback;

        DOTween.Clear(true);
        DOTween.ClearCachedTweens();
    }

    protected override void Awake()
    {
        base.Awake();
        Events.PlayerLost += GameOver;
        Events.GameEnded += Win;
        Events.EnemyDied += EnemyDied;
        Events.EnemyGotHit += EnemyGotHitCallback;
        Events.BossHit += BossEnemyHitCallback;

        Game.UseSlowMo = false;

        Application.targetFrameRate = 60;

        GameManager.Instance.ChangeGameState(GameStateEnum.GAME);

        baseGameMode = GameObject.FindObjectOfType<BaseGameMode>();

        if (EnemyWaypoints != null) Instantiate(EnemyWaypoints, transform, false);
        if (AsteroidBackgroundSpawner != null && HasAsteroids) Instantiate(AsteroidBackgroundSpawner, transform, false);
        if (Tutorial != null) Instantiate(Tutorial, transform, false);
    }

    void Start()
    {
        playerData = PersistantData.GetPlayerData();
        playerData.SetSuperMeter(0);
        playerData.ResetWeaponPowerUPCollected();

        playerShipData = playerData.GetCurrentPlayerShipData();

        SetGameState(GameState.START);
    }


    IEnumerator StartGameDelay()
    {
        Game.Reset();
        yield return new WaitForSeconds(1.0f);

        if (PlayerManager.GetPlayer() == null)
        {
            int shipSelected = playerData.CurrrentSelectedShip;
            player = PlayerManager.CreatePlayer(shipSelected);
            playerShip = player.GetComponentInChildren<PlayerShip>();
        }

        playerShip.DisableFire();
        yield return new WaitForSeconds(2.0f);
        playerShip.EnableFire();
        SetGameState(GameState.GAME);
    }

    IEnumerator DelayGameOver()
    {
        Time.timeScale = 1.0f;

        playerShipData.Upgrades[(int)UpgradeTypeEnum.Shield] = 0;

        playerData.SetScore(Game.Score);
        playerData.Coins += Game.CoinPicked;

        AchievementSystem.instance.Report(10, Game.EnemyKilled);
        AchievementSystem.instance.Report(11, Game.EnemyKilled);

        SaveSystem.SaveGame();

        AudioManager.PlayMusic("GameOver", false);

        yield return new WaitForSeconds(2.0f);

        Events.OnGameOver?.Invoke(this);


    }

    IEnumerator DelayWinScreen()
    {
        Time.timeScale = 1.0f;

        playerShipData.Upgrades[(int)UpgradeTypeEnum.Shield] = 0;

        GameManager.Instance.UpdatePlayerStatistics();

        SaveSystem.SaveGame();

        yield return new WaitForSeconds(2.0f);

        AudioManager.PlayMusic("Victory", false);

        playerShip?.Exit();

        yield return new WaitForSeconds(2.0f);
        Events.OnWin?.Invoke(this);
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


    //Callbacks
    public virtual void EnemyDiedCallback(string id, Enemy baseEnemy)
    {
        if (baseEnemy.Id.Equals(baseEnemy.Id))
        {
            playerData.SetSuperMeter(playerData.PowerUpLevel + 0.025f);


            int PlayerLevel = playerData.GetCurrentPlayerShipData().level;
            int EnemyLevel = baseEnemy.Level;
            int levelDiffrence = PlayerLevel / EnemyLevel;

            if (levelDiffrence == 0)
            {
                levelDiffrence = 1;
            }

            float XPEarned = (2.5f * PlayerLevel) / levelDiffrence;
            playerData.EarnXP(XPEarned);

            GuiManager.CreateFloatingText("<color=" + "yellow" + ">" + XPEarned + "</color>" + "<color=" + "orange" + "> XP </color>", baseEnemy.transform.localPosition);


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

            GameManager.Instance.PlayerQuestProgress(ObjectiveTypeEnum.KILL, 1);
            GameManager.Instance.PlayerQuestProgress(ObjectiveTypeEnum.SCORE, score);
        }
    }

    private void EnemyDied(string name, Enemy baseEnemy)
    {
        if (baseEnemy.Id.Equals(name))
        {
            Game.NumberOfEnemies--;

            int PlayerLevel = playerData.GetCurrentPlayerShipData().level;
            int EnemyLevel = baseEnemy.Level;
            int levelDiffrence = PlayerLevel / EnemyLevel;

            if (levelDiffrence == 0)levelDiffrence = 1;


            float XPEarned = (2.5f * PlayerLevel) / levelDiffrence;
            playerData.EarnXP(XPEarned);

            GuiManager.CreateFloatingText("<color=" + "yellow" + ">" + XPEarned + "</color>" + "<color=" + "orange" + "> XP </color>", baseEnemy.transform.localPosition);


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
            GameManager.Instance.PlayerQuestProgress(ObjectiveTypeEnum.KILL, kills);
        }

    }

    
    public void Win()
    {
        SetGameState(GameState.WIN);
    }

    public void GameOver()
    {
        SetGameState(GameState.GAMEOVER);
    }

    //Callbacks
    public virtual void BossDiedCallback(string id, BossEnemy baseEnemy)
    {
        if (baseEnemy.Id.Equals(id))
        {
            int BossId = id[id.Length - 1];
            GameManager.Instance.PlayerQuestProgress(ObjectiveTypeEnum.BOUNTY, BossId);
        }
    }

    public void EnemyGotHitCallback(string id, Enemy baseEnemy)
    {
        playerData.SetSuperMeter(playerData.PowerUpLevel + 0.01f);
    }


    public void BossEnemyHitCallback(string id, BossEnemy bossEnemy)
    {
        playerData.SetSuperMeter(playerData.PowerUpLevel + 0.01f);
    }

    public void EnemyEscapedCallback(string id, Enemy baseEnemy)
    {
        if (baseEnemy.Id.Equals(id))
        {
            Game.EnemyEscaped++;
        }
    }
    
    public void BossGotHit(string id, Enemy baseEnemy)
    {
        if (baseEnemy.Id.Equals(id))
        {
            playerData.SetSuperMeter(playerData.PowerUpLevel + 0.025f);
        }
    }
    
    //Getters
    public static BaseGameMode GetGameMode()
    {
        return Instance.baseGameMode;
    }



}
