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

    protected override void OnCleanup()
    {
        base.OnCleanup();

        playerShip.GetComponent<PlayerShip>().PlayerShipDeath -= PlayerShipCallback;
    }


    void Start()
    {
        Application.targetFrameRate = 60;

        if (AudioManager.Instance)
            AudioManager.PlayRandomMusic(true);


        if (PlayerManager.GetPlayer() == null)
        {
            int shipSelected = GameManager.Instance.GetPlayerData().currentSelectedShip;
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
        AudioManager.PlayMusic("GameOver", false);
        OnGameOver?.Invoke(this);

    }
    IEnumerator DelayWinScreen()
    {
        PlayerData playerData = GameManager.Instance.GetPlayerData();
        ObjectiveData objectiveData = playerData.GetOnGoingObjectiveById(ObjectiveType.Unharmed);

        if (objectiveData != null)
            objectiveData.UpdateProgress(1);

        playerData.Coins += GameSession.CoinEarnInGame;
        playerData.m_EnemyKilled += GameSession.CurrentEnemyKilled;

        yield return new WaitForSeconds(4.0f);

        PlayerShip playerShip = PlayerManager.GetPlayer();
        playerShip.Exit();

    
        OnWin?.Invoke(this);
    }

}
