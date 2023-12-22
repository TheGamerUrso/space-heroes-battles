using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
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
            var gameControllerFound = GameObject.FindObjectOfType<GameController>();
            if (gameControllerFound == null)
            {
                return GameState.GAME;
            }
            return gameControllerFound.currentGameState;
        }
    }

    public static GameController Instance
    {
        get
        {
            var gameControllerFound = GameObject.FindObjectOfType<GameController>();
            if (gameControllerFound == null)
            {
                return null;
            }
            return gameControllerFound;
        }
    }
    [Header("Components")]
    [SerializeField] private GameObject EnemyWaypoints;
    public GameObject AsteroidBackgroundSpawner;
    public GameObject Tutorial;
    public GameObject GUI;

    [Header("Config")]
    public bool HasAsteroids;


    private Ship playerShip;
    private PlayerData playerData;
    private BaseGameMode baseGameMode;


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
    public void OnDestroy()
    {       
        Events.PlayerLost -= GameOver;
        Events.GameEnded -= Win;

        DOTween.Clear(true);
        DOTween.ClearCachedTweens();
    }

    protected void Awake()
    {      
        Events.PlayerLost += GameOver;
        Events.GameEnded += Win;

        Game.UseSlowMo = false;
        Application.targetFrameRate = 60;

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
        SetGameState(GameState.START);
    }
    
    IEnumerator StartGameDelay()
    {        
        GameManager.Instance.ChangeGameState(GameStateEnum.GAME);
        Game.NewGame();
        yield return new WaitForSeconds(1.0f);

        if (PlayerManager.GetPlayer() == null)
        {
            int shipSelected = playerData.CurrrentSelectedShip;
            var player = PlayerManager.CreatePlayer(shipSelected);
            playerShip = player.GetComponentInChildren<PlayerShip>();
        }

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
                    baseGameMode.GameOver();
                }
                break;
            case GameState.WIN:
                if (!Game.IsGameOver)
                {
                    Game.IsGameOver = true;
                    baseGameMode.Win();
                }
                break;
        }
        currentGameState = gameState;
    }

    public void Win()
    {
        SetGameState(GameState.WIN);
    }

    public void GameOver()
    {
        SetGameState(GameState.GAMEOVER);
    }

    public static BaseGameMode GetGameMode()
    {
        return Instance.baseGameMode;
    }
}
