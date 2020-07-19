using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameController : MonoSingleton<GameController>
{
    public enum GameState
    {
        START, GAME, GAMEOVER, WIN, RESULTS
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
        if (Application.platform == RuntimePlatform.Android)
        {
            //if (!focus && !Game.IsGameOver)
            //{
            //    GameManager.Instance.PauseTheGame(true);
            //}
        }
    }

    private void OnApplicationPause(bool Paused)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            //if (Game.IsGameOver == false)
            //{
            //    GameManager.Instance.PauseTheGame(true);
            //}
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
        if (AudioManager.Instance)
            AudioManager.PlayRandomMusic(true);

        Game.Reset();

        if (PlayerManager.GetPlayer() == null)
        {
            int shipSelected = playerData.currentSelectedShip;
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
            case GameState.RESULTS:
                Events.OnGameOver?.Invoke(this);
                break;
            default:
                break;
        }
        currentGameState = gameState;
    }

    public GameState GetGameState()
    {
        return currentGameState;
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



        playerData.m_EnemyKilled = kills;
        ObjectiveData objectiveData = playerData.GetOnGoingObjectiveById(ObjectiveType.Kill);

        if (objectiveData != null)
        {
            var newProgress = objectiveData.progress + playerData.m_EnemyKilled;
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


    public void UpdateAchievements()
    {
        if (GooglePlayServicesManager.GetInitialized())
        {
            GooglePlayServicesManager.ReportAchivementProgress(EasyMobile.EM_GameServicesConstants.Achievement_Piece_of_Cake, playerData.TotalKills);
            GooglePlayServicesManager.ReportAchivementProgress(EasyMobile.EM_GameServicesConstants.Achievement_Destroyer, playerData.TotalKills);
        }
    }

    public void GameOver()
    {
        SetGameState(GameState.GAMEOVER);
    }

    IEnumerator DelayGameOver()
    {
        Time.timeScale = 1.0f;

        baseGameMode.GameOver();

        playerShipData.Upgrades[(int)UpgradeTypeEnum.Shield] = 0;

        SaveSystem.SaveGame();

        if (Game.IsSurvivalMode)
        {
            int levelIndex = GameManager.LevelIndexSelected;
            playerData.SetScore(levelIndex, Game.Score);
        }

        yield return new WaitForSeconds(2.0f);

        AudioManager.PlayMusic("GameOver", false);


        SetGameState(GameState.RESULTS);
    }
    IEnumerator DelayWinScreen()
    {
        Time.timeScale = 1.0f;
        playerData.PlayedGame = true;
        playerShipData.Upgrades[(int)UpgradeTypeEnum.Shield] = 0;

        playerData.Coins += Game.CoinPicked;
        playerData.m_EnemyKilled += Game.EnemyKilled;

        PlayerChallengesCheck();

        PlayerQuestCheck();

        UpdateAchievements();

        SaveSystem.SaveGame();

        yield return new WaitForSeconds(2.0f);

        playerShip?.Exit();

        Events.OnWin?.Invoke(this);
    }


    public void UnlockNextMission()
    {
        Dictionary<string, LevelObjectiveData[]> Challanges = playerData.GetListOfObjectives();
        int missionsCompleted = 1;
        foreach (KeyValuePair<string, LevelObjectiveData[]> item in Challanges)
        {
            if (item.Value[0].completed == true)
            {
                missionsCompleted++;
            }
        }
        int levelIndex = GameManager.LevelIndexSelected;
#if UNITY_ANDROID
        if (GooglePlayServicesManager.GetInitialized())
        {
            GooglePlayServicesManager.UnlockAchievement(levelIndex);
        }
#elif UNITY_EDITOR
     Debug.Log("UnlockAchievement"); 
#endif
        if (missionsCompleted > 9)
        {
            if (!playerData.SurvivalUnlocked)
            {
                playerData.SurvivalUnlocked = true;
            }
        }
        playerData.LevelUnlocked = missionsCompleted;
    }

    public void PlayerChallengesCheck()
    {
        int levelIndex = GameManager.LevelIndexSelected;
        playerData.SetScore(levelIndex, Game.Score);
        var levelName = "Level" + levelIndex;
        var killed = Game.EnemySpawnInTotal * .9f;
        var collected = Game.EnemySpawnInTotal * .9f;
        var missionCollection = PersistantData.GetMissionCollection();

        Scene scene = SceneManager.GetActiveScene();
        string index = scene.name[scene.name.Length - 1].ToString();
        Mission mission = missionCollection.GetMission(int.Parse(index));

        var levelObjectiveDatas = playerData.GetLevelObjectives(levelName);

        if (levelObjectiveDatas[0].completed == false)
        {
            levelObjectiveDatas[0].completed = true;
            playerData.EarnXP(20 * playerData.GetCurrentPlayerShipData().level);
        }

        float enemyKilled = Game.EnemyKilled;

        if (!levelObjectiveDatas[1].completed && enemyKilled >= killed)
        {
            levelObjectiveDatas[1].completed = true;
            playerData.EarnXP(30 * playerData.GetCurrentPlayerShipData().level);
        }

        if (!levelObjectiveDatas[2].completed && playerData.PlayedGame && !playerData.GotHitInGame)
        {
            levelObjectiveDatas[2].completed = true;
            playerData.EarnXP(40 * playerData.GetCurrentPlayerShipData().level);
        }

        float coinEarnInGame = Game.CoinPicked;

        if (!levelObjectiveDatas[3].completed && coinEarnInGame >= 0 && coinEarnInGame >= collected)
        {
            levelObjectiveDatas[3].completed = true;
            playerData.EarnXP(10 * playerData.GetCurrentPlayerShipData().level);
        }

        UnlockNextMission();
    }

    public void PlayerQuestCheck()
    {
        for (int i = 0; i < playerData.ListOfOnGoingObjectives.Count; i++)
        {
            ObjectiveData objective = playerData.ListOfOnGoingObjectives[i];
            switch ((ObjectiveType)objective.objectiveType)
            {
                //case ObjectiveType.Use:
                //    if (objective.completed == false)
                //    {
                //        objective.UpdateProgress(playerData.superUsed);
                //    }
                //    break;
                case ObjectiveType.Unharmed:
                    if (objective.completed == false)
                    {
                        if (playerData.GotHitInGame == false)
                        {
                            ObjectiveData objectiveData = playerData.GetOnGoingObjectiveById(ObjectiveType.Unharmed);
                            objectiveData.UpdateProgress(1);
                        }
                    }
                    break;
                case ObjectiveType.survive:
                    var surviveProgress = objective.progress;
                    surviveProgress++;
                    objective.UpdateProgress(surviveProgress);
                    break;
            }
        }
    }

    private void Update()
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
    }
}
