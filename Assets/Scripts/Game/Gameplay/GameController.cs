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

    private float slowMo;
    private float delayTheSlowMoEffectTimer;


    protected override void OnAwake()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name.Equals("boot"))
            {
                Debug.Log("boot found skip");
                return;
            }
            Debug.Log("Boot not found Loading");
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

        float XPEarned = (2.5f * PlayerLevel) / levelDiffrence;
        playerData.EarnXP(XPEarned);
        playerData.PowerUpLevel += .1f;


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

    public void SetPlayerData()
    {
        playerData.Coins += 0;
        playerData.TotalKills += 0;
    }

    public void GameOver()
    {
        if (GameSession.IsGameOver == false)
        {
            Time.timeScale = 1.0f;

            GameSession.IsGameOver = true;

            spawn.GameOver();

            playerShipData.Upgrades[((int)UpgradeType.Shield - 1)] = 0;

            SetPlayerData();

            SaveSystem.SaveGame();

            UpdateAchievements();

            StartCoroutine(DelayGameOver());
        }
    }

    IEnumerator DelayGameOver()
    {
        yield return new WaitForSeconds(4.0f);
        AudioManager.PlayMusic("GameOver", false);
        OnGameOver?.Invoke(this);

    }
    IEnumerator DelayWinScreen()
    {
        Time.timeScale = 1.0f;

        playerShipData.Upgrades[((int)UpgradeType.Shield - 1)] = 0;
        PlayerChallengesCheck();

        PlayerQuestCheck();

        UpdateAchievements();

        yield return new WaitForSeconds(4.0f);

        PlayerShip playerShip = PlayerManager.GetPlayer();
        playerShip.Exit();


        OnWin?.Invoke(this);
    }


    public void UnlockNextMission()
    {
        Dictionary<string, LevelObjectiveData[]> Challanges = playerData.GetListOfObjectives();
        int missionsCompleted = 0;
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
        playerData.Coins += GameSession.CoinEarnInGame;
        playerData.m_EnemyKilled += GameSession.CurrentEnemyKilled;

        int levelSelected = (levelIndex + 1);
        var levelName = "Level" + levelSelected;
        var killed = GameSession.EnemySpawnInTotal * .9f;
        var collected = GameSession.EnemySpawnInTotal * .9f;
        var missionCollection = PersistantData.GetMissionCollection();
        var mission = missionCollection.GetMission(levelSelected);

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

        if (!levelObjectiveDatas[2].completed && playerData.PlayedGame && playerData.GotHitInGame == false)
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

        if (num == 4)
        {

            if (GooglePlayServicesManager.Instance)
            {
                GooglePlayServicesManager.Instance.UnlockAchievement(GameManager.LevelIndexSelected);
            }

        }
#elif UNITY_EDITOR
     Debug.Log("UnlockAchievement"); 
#endif

        UnlockNextMission();
    }

    public void PlayerQuestCheck()
    {
        playerData.PlayedGame = true;

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
}
