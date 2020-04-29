using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameController : Singleton<GameController>
{
    public static Action<GameController> OnGameOver;
    public static Action<GameController> OnWin;

    GameObject player;
    PlayerShip playerShip;
    PlayerData playerData;
    PlayerShipData playerShipData;

    private SpawnEnemies spawn;

    private float delayTheSlowMoEffectTimer = .3f;
    private float delay = 2;

    private void OnApplicationFocus(bool focus)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (!focus && GameSession.IsGameOver == false)
            {
                GameManager.Instance.PauseTheGame(focus);
            }
        }
    }

    private void OnApplicationPause(bool Paused)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (GameSession.IsGameOver == false)
            {
                GameManager.Instance.PauseTheGame(Paused);
            }
        }
    }

    protected override void OnAwake()
    {
        /**
         * Only For Editor
         */

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name.Equals("boot"))
            {
              //  Debug.Log("boot found skip");
                return;
            }
           // Debug.Log("Boot not found Loading");
            SceneManager.LoadScene("boot", LoadSceneMode.Additive);
        }
    }

    protected override void OnCleanup()
    {
        base.OnCleanup();

        playerShip.PlayerShipDeath -= PlayerShipCallback;
        spawn.SpawnEnded -= Win;
        spawn.EnemyDied -= EnemyDied;
    }


    void Start()
    {
        Application.targetFrameRate = 60;

        if (AudioManager.Instance)
            AudioManager.PlayRandomMusic(true);


        if (PlayerManager.GetPlayer() == null)
        {
            playerData = PersistantData.GetPlayerData();
            int shipSelected = playerData.currentSelectedShip;
            player = PlayerManager.CreatePlayer(shipSelected);
        }

        playerShip = player.GetComponent<PlayerShip>();
        playerShip.PlayerShipDeath += PlayerShipCallback;

        playerData = PersistantData.GetPlayerData();
        playerShipData = playerData.GetCurrentPlayerShipData();

        spawn = GameObject.FindObjectOfType<SpawnEnemies>();
        spawn.SpawnEnded += Win;
        spawn.EnemyDied += EnemyDied;

        spawn.InitReference(playerData, playerShip);

        GameSession.Reset();
    }

    private void EnemyDied(BaseEnemy baseEnemy)
    {
        int PlayerLevel = playerData.GetCurrentPlayerShipData().level;
        int EnemyLevel = baseEnemy.Level;
        int levelDiffrence = PlayerLevel / EnemyLevel;

        if (levelDiffrence == 0)
        {
            levelDiffrence = 1;
        }

        if (!GameSession.SurvivalMode)
        {
            float XPEarned = (2.5f * PlayerLevel) / levelDiffrence;
            playerData.EarnXP(XPEarned);
        }

        playerData.PowerUpLevel += .025f;


        int score = GameSession.Multiplier * baseEnemy.m_ValueOfEnemy;
        GameSession.CurrentEnemyKilled++;
        GameSession.enemyKilled++;
        GameSession.Score = score;
        GameSession.Multiplier++;
    }

    private void PlayerShipCallback()
    {
        GameOver();
    }

    public void Win()
    {
        if (!GameSession.IsGameOver)
        {
            GameSession.IsGameOver = true;

            StartCoroutine(DelayWinScreen());
        }
    }


    public void UpdateAchievements()
    {
        if (GooglePlayServicesManager.Instance)
        {
            GooglePlayServicesManager.Instance.ReportAchivementProgress(EasyMobile.EM_GameServicesConstants.Achievement_Piece_of_Cake, playerData.TotalKills);
            GooglePlayServicesManager.Instance.ReportAchivementProgress(EasyMobile.EM_GameServicesConstants.Achievement_Destroyer, playerData.TotalKills);
        }
    }

    public void GameOver()
    {
        if (GameSession.IsGameOver == false)
        {
            GameSession.IsGameOver = true;
            StartCoroutine(DelayGameOver());
        }
    }

    IEnumerator DelayGameOver()
    {
        Time.timeScale = 1.0f;

        spawn.GameOver();

        playerShipData.Upgrades[(int)UpgradeType.Shield] = 0;

        GameSession.CoinEarnInGame = 0;
        GameSession.CurrentEnemyKilled = 0;

        SaveSystem.SaveGame();

        yield return new WaitForSeconds(2.0f);

        AudioManager.PlayMusic("GameOver", false);

        OnGameOver?.Invoke(this);

    }
    IEnumerator DelayWinScreen()
    {
        Time.timeScale = 1.0f;
        playerData.PlayedGame = true;
        playerShipData.Upgrades[(int)UpgradeType.Shield] = 0;

        playerData.Coins += GameSession.CoinEarnInGame;
        playerData.m_EnemyKilled += GameSession.CurrentEnemyKilled;

        PlayerChallengesCheck();

        PlayerQuestCheck();

        UpdateAchievements();

        SaveSystem.SaveGame();

        yield return new WaitForSeconds(2.0f);

        PlayerShip playerShip = PlayerManager.GetPlayer();
        playerShip.Exit();


        OnWin?.Invoke(this);
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
        playerData.LevelUnlocked = missionsCompleted;
    }

    public void PlayerChallengesCheck()
    {
        int levelIndex = GameManager.LevelIndexSelected;
        playerData.SetScore(levelIndex, GameSession.score);
        int levelSelected = levelIndex;
        var levelName = "Level" + levelSelected;
        var killed = GameSession.EnemySpawnInTotal * .9f;
        var collected = GameSession.EnemySpawnInTotal * .9f;
        var missionCollection = PersistantData.GetMissionCollection();
        var mission = missionCollection.GetMission(levelSelected - 1);

        var levelObjectiveDatas = playerData.GetLevelObjectives(levelName);

        if (levelObjectiveDatas[0].completed == false)
        {
            levelObjectiveDatas[0].completed = true;
            playerData.EarnXP(20 * playerData.GetCurrentPlayerShipData().level);
        }

        float enemyKilled = GameSession.CurrentEnemyKilled;

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

        float coinEarnInGame = GameSession.coinEarnInGame;

        if (!levelObjectiveDatas[3].completed && coinEarnInGame >= 0 && coinEarnInGame >= collected)
        {
            levelObjectiveDatas[3].completed = true;
            playerData.EarnXP(10 * playerData.GetCurrentPlayerShipData().level);
        }

#if UNITY_ANDROID
        int num = 0;

        for (int i = 0; i < levelObjectiveDatas.Length; i++)
        {
            if (levelObjectiveDatas[i].completed)
            {
                num++;
            }
        }

        if (GooglePlayServicesManager.Instance)
        {
            GooglePlayServicesManager.Instance.UnlockAchievement(levelSelected);
        }


#elif UNITY_EDITOR
     Debug.Log("UnlockAchievement"); 
#endif

        UnlockNextMission();
    }

    public void PlayerQuestCheck()
    {


        for (int i = 0; i < playerData.ListOfOnGoingObjectives.Count; i++)
        {
            ObjectiveData objective = playerData.ListOfOnGoingObjectives[i];
            switch ((ObjectiveType)objective.objectiveType)
            {
                case ObjectiveType.Kill:
                    if (objective.completed == false)
                    {
                        var progressSoFar = objective.progress + 0;
                        objective.UpdateProgress(progressSoFar);
                    }

                    break;
                case ObjectiveType.Use:
                    if (objective.completed == false)
                    {
                        objective.UpdateProgress(playerData.superUsed);
                    }
                    break;
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
                case ObjectiveType.spend:
                    break;
                default:
                    break;
            }
        }
    }

    private void Update()
    {
        if (delay > 0)
        {
            delay -= Time.deltaTime;
        }
        else
        {
            if (!GameSession.IsGameOver && !GameManager.Paused)
            {
                if (GameSession.useSloMo)
                {
                    Time.timeScale = delayTheSlowMoEffectTimer;
                }
                else if (!GameSession.useSloMo && Time.timeScale < 1)
                {
                    Time.timeScale = 1.0f;
                }
            }
        }
    }
}
