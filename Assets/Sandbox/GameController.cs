using System;
using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.PoolSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : Singleton<GameController>
{
    private GameObject playerShip;

    public static bool IsGameOver;
    public static bool useSloMo;

    public int enemyKilled;
    public int enemyEscaped;

    private int Wave;
    private int MaxWave;
    private int TotalEnemies;


    public int EnemySpawnInTotal { get; set; }
    public int LevelDifficulty { get; set; }
    public float Score { get; set; }
    public int WaveSurvived { get; set; }
    public int EnemyKilled { get; set; }
    public int CurrentEnemyKilled { get; set; }
    public int CoinDropInTotal { get; set; }
    public int counsEarnInGame { get; set; }


    private float slowMo;

    private float delayTheSlowMoEffectTimer;

    public void GameStatsChanged(int Wave, int MaxWave, int TotalEnemies)
    {
        this.Wave = Wave;
        this.MaxWave = MaxWave;
        this.TotalEnemies = TotalEnemies;
    }

    protected override void OnAwake()
    {
        Scene bootScene = SceneManager.GetSceneByName("boot");
        if (!bootScene.isLoaded)
        {
            SceneManager.LoadScene("boot", LoadSceneMode.Additive);
        }
    }

    void Start()
    {
        IsGameOver = false;

        Application.targetFrameRate = 60;

        if (AudioManager.Instance)
            AudioManager.PlayRandomMusic(true);

        delayTheSlowMoEffectTimer = 4;

        if (SpawnEnemies.Instance)
        {
            SpawnEnemies.Instance.GameStatsChanged += GameStatsChanged;
            SpawnEnemies.Instance.BossDied += BossEnemyCallback;

            EnemySpawnInTotal = SpawnEnemies.Instance.TotalEnemies;

        }

        if (PlayerManager.GetPlayer() == null)
        {
            int shipSelected = GameManager.CurrentHeroChoosen;
            playerShip = PlayerManager.CreatePlayer(shipSelected);
        }

        EnemySpawnInTotal = TotalEnemies;
     
        playerShip.GetComponent<PlayerShip>().PlayerShipDeath += PlayerShipCallback;


    }


    private void PlayerShipCallback()
    {
        GameOver();
    }

    private void BossEnemyCallback(string id, BaseEnemy bossEnemy)
    {
        Win();
    }

    public void ToggleSlowMo(bool value)
    {
        useSloMo = value;
        if (value == false)
        {
            Time.timeScale = 1.0f;
        }
    }


    private void Update()
    {
        if (!IsGameOver)
        {
            if (AudioManager.Instance)
            {
                if (AudioManager.Instance.MusicIsDone())
                {
                    AudioManager.PlayRandomMusic();
                }
            }
            SlowMoEffect();
        }
    }

    IEnumerator DelayGameOver()
    {
        yield return new WaitForSeconds(4.0f);
        GuiManager.Instance.GameOver();

    }
    IEnumerator DelayWinScreen()
    {
        yield return new WaitForSeconds(4.0f);
        GuiManager.Instance.Win();

    }

    public void SlowMoEffect()
    {
#if UNITY_ANDROID
        if (useSloMo && !GameManager.Paused)
        {
            if (Input.touchCount > 0 || Input.GetMouseButton(0))
            {
                //     slowMo = 1;
            }
            else
            {
                //       slowMo = .3f;

            }
            // Time.timeScale = slowMo;
        }
#endif
    }


    public void Win()
    {
        if (!IsGameOver)
        {
            IsGameOver = true;
            StartCoroutine(DelayWinScreen());
        }
    }
    public void GameOver()
    {
        if (IsGameOver == false)
        {
            IsGameOver = true;
            StartCoroutine(DelayGameOver());
        }
    }

    public void EnemyGotHit(string id, BaseEnemy enemy)
    {

    }

    public void EnemyEscaped(string id, BaseEnemy enemy)
    {
        enemyEscaped++;
    }

    public void EnemyDied(string id, BaseEnemy enemy)
    {
        Vector3 enemyPos = enemy.transform.position;

        enemyKilled++;

        PlayerData playerData = DataController.GetPlayerData();
        ObjectiveData objectiveData = playerData.GetOnGoingObjectiveById(ObjectiveType.Kill);

        if (objectiveData != null)
            objectiveData.UpdateProgress(CurrentEnemyKilled);

        //Update ComboKillIndicator
        if (ComboKillIndicator.instance)
            ComboKillIndicator.instance.ConfirmKill();

        int multiplayer = 0;
        if (ComboKillIndicator.instance)
        {
            multiplayer = ComboKillIndicator.instance.GetMultiplier();
        }

        //Update Score
        int score = multiplayer * enemy.m_ValueOfEnemy;
        Score += score;

        if (GuiManager.Instance)
        {
            GuiManager.Instance.UpdateScore(score);
            GuiManager.CreateFloatingText(string.Format("{0}", score), enemyPos);
        }

        //Update Player Attributes
        PlayerShip playerShip = PlayerManager.GetPlayer();
        if (playerShip != null)
        {
            int PlayerLevel = playerShip.Level;
            int EnemyLevel = enemy.Level;
            int levelDiffrence = PlayerLevel / EnemyLevel;

            if (levelDiffrence == 0)
            {
                levelDiffrence = 1;
            }

            // float XPEarned = (2.5f * PlayerLevel) / levelDiffrence;
            float XPEarned = 5 / levelDiffrence;
            playerShip.AddXP((int)XPEarned);
            Debug.Log(string.Format("exp = {0}\n", XPEarned));


            playerShip.IncreasePowerUp(.1f);
        }

        DropController.PickRandomDropItem(enemy.transform);
    }
    public void SetLevelDifficuilty(int Level)
    {
        LevelDifficulty = Level;
    }

}
