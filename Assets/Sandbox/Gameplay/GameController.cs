using System;
using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.PoolSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameSession
{
    public static bool IsGameOver;
    public static int EnemySpawnInTotal { get; set; }

    public static float score;
    public static float Score
    {
        get
        {
            return score;
        }

        set
        {
            score = value; 
        }
    }

    public static int WaveSurvived { get; set; }

    private static int currentEnemyKilled;

    public static int CurrentEnemyKilled
    {
        get
        {
            return currentEnemyKilled;
        }

        set
        {

            currentEnemyKilled = value;
            PlayerData playerData = DataController.GetPlayerData();
            ObjectiveData objectiveData = playerData.GetOnGoingObjectiveById(ObjectiveType.Kill);
            if (objectiveData != null)
                objectiveData.UpdateProgress(currentEnemyKilled);
        }
    }
   
    public static int counsEarnInGame { get; set; }

    private static int superUsed;
    public static int SuperUsed
    {

        get
        {
            return superUsed;
        }

        set
        {
            superUsed = value;
            PlayerData playerData = DataController.GetPlayerData();
            ObjectiveData objectiveData = playerData.GetOnGoingObjectiveById(ObjectiveType.Use);
            if (objectiveData != null)
                objectiveData.UpdateProgress(superUsed);
        }
    }

    private static bool getDamaged;

    public static bool GotDamaged
    {
        get
        {
            return getDamaged;
        }

        set
        {
            getDamaged = value;
            PlayerData playerData = DataController.GetPlayerData();
            ObjectiveData objectiveData = playerData.GetOnGoingObjectiveById(ObjectiveType.Unharmed);
            if (objectiveData != null)
                objectiveData.UpdateProgress(1);
        }
    }

    public static int multiplier { get; set; }

    public static int enemyKilled;
    public static int enemyEscaped;

    public static void Reset()
    {
        IsGameOver = false;
        EnemySpawnInTotal = 0;
        Score = 0;
        WaveSurvived = 0;
        currentEnemyKilled = 0;
        currentEnemyKilled = 0;
        counsEarnInGame = 0;
        superUsed = 0;
        getDamaged = false;
        multiplier = 0;

        enemyKilled = 0;
        enemyEscaped = 0;
    }
}

public static class GameLevel
{   
    public static bool useSloMo;
   
    public static int LevelDifficulty { get; set; }
    public static int EnemyKilled { get; set; }
    public static int CoinDropInTotal { get; set; }
}

public class GameController : Singleton<GameController>
{
    public static Action<GameController> OnGameOver;
    public static Action<GameController> OnWin;

    private GameObject playerShip;

    private float slowMo;
    private float delayTheSlowMoEffectTimer;


    protected override void OnAwake()
    {
#if UNITY_EDITOR
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
#endif
    }


    void Start()
    {
        Application.targetFrameRate = 60;

        if (AudioManager.Instance)
            AudioManager.PlayRandomMusic(true);


        if (PlayerManager.GetPlayer() == null)
        {
            int shipSelected = GameManager.CurrentHeroChoosen;
            playerShip = PlayerManager.CreatePlayer(shipSelected);
        }

        playerShip.GetComponent<PlayerShip>().PlayerShipDeath += PlayerShipCallback;


        SpawnEnemies spawn = GameObject.FindObjectOfType<SpawnEnemies>();
        spawn.SpawnEnded = () => { Win(); };
        
        GameSession.Reset();

    }

    private void PlayerShipCallback()
    {
        GameOver();
    }

    //    private void BossEnemyCallback(string id, BaseEnemy bossEnemy)
    //    {
    //        Win();
    //    }

    //    public void ToggleSlowMo(bool value)
    //    {
    //        useSloMo = value;
    //        if (value == false)
    //        {
    //            Time.timeScale = 1.0f;
    //        }
    //    }


    //    private void Update()
    //    {
    //        if (!IsGameOver)
    //        {
    //            if (AudioManager.Instance)
    //            {
    //                if (AudioManager.Instance.MusicIsDone())
    //                {
    //                    AudioManager.PlayRandomMusic();
    //                }
    //            }
    //            SlowMoEffect();
    //        }
    //    }

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

          

            StartCoroutine(DelayGameOver());
        }
    }

    IEnumerator DelayGameOver()
    {
        yield return new WaitForSeconds(4.0f);
        OnGameOver?.Invoke(this);

    }
    IEnumerator DelayWinScreen()
    {
        PlayerShip playerShip = PlayerManager.GetPlayer();
        playerShip.Exit();

        yield return new WaitForSeconds(4.0f);

        PlayerData playerData = DataController.GetPlayerData();
        ObjectiveData objectiveData = playerData.GetOnGoingObjectiveById(ObjectiveType.Unharmed);
        if (objectiveData != null)
            objectiveData.UpdateProgress(1);

        playerData.Coins += GameSession.counsEarnInGame;
        playerData.m_EnemyKilled += GameSession.CurrentEnemyKilled;


        playerData.Save();
        OnWin?.Invoke(this);
    }

}
