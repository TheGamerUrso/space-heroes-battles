using System;
using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.PoolSystem;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameController : Singleton<GameController>
{
    public static Action<GameController> OnGameOver;
    public static Action<GameController> OnWin;

    private GameObject player;
    private PlayerData playerData;
    private PlayerShip playerShip;


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
            playerData = GameManager.Instance.GetPlayerData();
            int shipSelected = playerData.currentSelectedShip;
            player = PlayerManager.CreatePlayer(shipSelected);
        }

        playerShip = player.GetComponent<PlayerShip>();
        playerShip.PlayerShipDeath += PlayerShipCallback;


        spawn = GameObject.FindObjectOfType<SpawnEnemies>();
        spawn.SpawnEnded += Win;
        spawn.EnemyDied += EnemyDied;

        GameSession.Reset();


        spawn.InitReference(playerData, playerShip);
    }

    private void EnemyDied(BaseEnemy baseEnemy)
    {
        int PlayerLevel = playerShip.Level;
        int EnemyLevel = baseEnemy.level;
        int levelDiffrence = PlayerLevel / EnemyLevel;

        if (levelDiffrence == 0)
        {
            levelDiffrence = 1;
        }

        float XPEarned = (2.5f * PlayerLevel) / levelDiffrence;
        playerShip.AddXP(XPEarned);
        playerShip.IncreasePowerUp(.1f);

        ////int dif = baseEnemy.level - playerShip.level;
        ////if (dif > 0)
        ////{
        ////    float level = 25 / baseEnemy.level;
        ////    playerShip.AddXP(baseEnemy.level);
        ////}

        int score = GameSession.Multiplier * baseEnemy.m_ValueOfEnemy;

        GameSession.CurrentEnemyKilled++;
        GameSession.enemyKilled++;
        GameSession.Score = score;
        Debug.Log("" + GameSession.score);
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
  
    public void GameOver()
    {
        if (GameSession.IsGameOver == false)
        {
            GameSession.IsGameOver = true;

            spawn.GameOver();

            Time.timeScale = 1.0f;

            PlayerShip playerShip = PlayerManager.GetPlayer();
            PlayerData playerData = GameManager.Instance.GetPlayerData();
            PlayerShipData playerShipData = playerData.GetCurrentPlayerShipData();

            playerShipData.Upgrades[((int)UpgradeType.Shield - 1)] = 0;
            playerShipData.level = playerShip.level;
            playerShipData.xp = playerShip.xp;
            playerShipData.xpToLevel = playerShip.xpToLevel;

            //TODO Coins Earn In Game
            //TODO Enemy Killed In Game
            playerData.Coins += 0;
            playerData.TotalKills += 0;

            SaveSystem.SaveGame();


            if (GooglePlayServicesManager.Instance)
            {
                GooglePlayServicesManager.Instance.ReportAchivementProgress(EasyMobile.EM_GameServicesConstants.Achievement_Piece_of_Cake, playerData.TotalKills);
                GooglePlayServicesManager.Instance.ReportAchivementProgress(EasyMobile.EM_GameServicesConstants.Achievement_Destroyer, playerData.TotalKills);
            }

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

        PlayerShip player = PlayerManager.GetPlayer();
        PlayerData playerData = GameManager.Instance.GetPlayerData();
        PlayerShipData playerShipData = playerData.GetCurrentPlayerShipData();

        playerShipData.Upgrades[((int)UpgradeType.Shield - 1)] = 0;

        if (player == null)
        {
            player = GameObject.FindObjectOfType<PlayerShip>();
        }

        playerShipData.level = player.level;
        playerShipData.xp = player.xp;
        playerShipData.xpToLevel = player.xpToLevel;
        //Save Game Data
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
                        if (player.IsPlayerDamaged == false)
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

        Dictionary<string, LevelObjectiveData[]> Challanges = playerData.GetListOfObjectives();
        int missionsCompleted = 0;
        foreach (KeyValuePair<string, LevelObjectiveData[]> item in Challanges)
        {
            if (item.Value[0].completed == true)
            {
                missionsCompleted++;
            }
        }
        int levelPlayed = GameManager.LevelSelected;

        playerData.SetScore(levelPlayed + 1, 0);

        playerData.LevelUnlocked = missionsCompleted;

        playerData.Coins += GameSession.CoinEarnInGame;
        playerData.m_EnemyKilled += GameSession.CurrentEnemyKilled;


        if (GooglePlayServicesManager.Instance)
        {
            GooglePlayServicesManager.Instance.ReportAchivementProgress(EasyMobile.EM_GameServicesConstants.Achievement_Piece_of_Cake, playerData.TotalKills);
            GooglePlayServicesManager.Instance.ReportAchivementProgress(EasyMobile.EM_GameServicesConstants.Achievement_Destroyer, playerData.TotalKills);
        }


        yield return new WaitForSeconds(4.0f);

        PlayerShip playerShip = PlayerManager.GetPlayer();
        playerShip.Exit();


        OnWin?.Invoke(this);
    }

}
