using System;
using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.PoolSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
public static class GameLevel
{
    public static bool IsGameOver;
    public static bool useSloMo;
    public static int enemyKilled;
    public static int enemyEscaped;

    public static int EnemySpawnInTotal { get; set; }
    public static int LevelDifficulty { get; set; }
    public static float Score { get; set; }
    public static int WaveSurvived { get; set; }
    public static int EnemyKilled { get; set; }
    public static int CurrentEnemyKilled { get; set; }
    public static int CoinDropInTotal { get; set; }
    public static int counsEarnInGame { get; set; }
}

public class GameController : Singleton<GameController>
{
    public static Action<GameController> OnGameOver;
    public static Action<GameController> OnWin;

    private GameObject playerShip;

    //    private float slowMo;

    //    private float delayTheSlowMoEffectTimer;

    //    public void GameStatsChanged(int Wave, int MaxWave, int TotalEnemies)
    //    {
    //        this.Wave = Wave;
    //        this.MaxWave = MaxWave;
    //        this.TotalEnemies = TotalEnemies;
    //    }

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
        if (!GameLevel.IsGameOver)
        {
            GameLevel.IsGameOver = true;

            StartCoroutine(DelayWinScreen());
        }
    }
    public void GameOver()
    {
        if (GameLevel.IsGameOver == false)
        {
            GameLevel.IsGameOver = true;
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
        yield return new WaitForSeconds(4.0f);
        OnWin?.Invoke(this);
    }

    //    public void SlowMoEffect()
    //    {
    //#if UNITY_ANDROID
    //        if (useSloMo && !GameManager.Paused)
    //        {
    //            if (Input.touchCount > 0 || Input.GetMouseButton(0))
    //            {
    //                //     slowMo = 1;
    //            }
    //            else
    //            {
    //                //       slowMo = .3f;

    //            }
    //            // Time.timeScale = slowMo;
    //        }
    //#endif
    //    }




    //    public void EnemyGotHit(string id, BaseEnemy enemy)
    //    {

    //    }

    //    public void EnemyEscaped(string id, BaseEnemy enemy)
    //    {
    //        enemyEscaped++;
    //    }

    //    public void EnemyDied(string id, BaseEnemy enemy)
    //    {
    //        Vector3 enemyPos = enemy.transform.position;

    //        enemyKilled++;

    //        PlayerData playerData = DataController.GetPlayerData();
    //        ObjectiveData objectiveData = playerData.GetOnGoingObjectiveById(ObjectiveType.Kill);

    //        if (objectiveData != null)
    //            objectiveData.UpdateProgress(CurrentEnemyKilled);

    //        //Update ComboKillIndicator
    //        if (ComboKillIndicator.instance)
    //            ComboKillIndicator.instance.ConfirmKill();

    //        int multiplayer = 0;
    //        if (ComboKillIndicator.instance)
    //        {
    //            multiplayer = ComboKillIndicator.instance.GetMultiplier();
    //        }

    //        //Update Score
    //        int score = multiplayer * enemy.m_ValueOfEnemy;
    //        Score += score;

    //        if (GuiManager.Instance)
    //        {
    //            GuiManager.Instance.UpdateScore(score);
    //            GuiManager.CreateFloatingText(string.Format("{0}", score), enemyPos);
    //        }

    //        //Update Player Attributes
    //        PlayerShip playerShip = PlayerManager.GetPlayer();
    //        if (playerShip != null)
    //        {
    //            int PlayerLevel = playerShip.Level;
    //            int EnemyLevel = enemy.Level;
    //            int levelDiffrence = PlayerLevel / EnemyLevel;

    //            if (levelDiffrence == 0)
    //            {
    //                levelDiffrence = 1;
    //            }

    //            // float XPEarned = (2.5f * PlayerLevel) / levelDiffrence;
    //            float XPEarned = 5 / levelDiffrence;
    //            playerShip.AddXP((int)XPEarned);
    //            Debug.Log(string.Format("exp = {0}\n", XPEarned));


    //            playerShip.IncreasePowerUp(.1f);
    //        }

    //        DropController.PickRandomDropItem(enemy.transform);
    //    }
    //    public void SetLevelDifficuilty(int Level)
    //    {
    //        LevelDifficulty = Level;
    //    }

}
