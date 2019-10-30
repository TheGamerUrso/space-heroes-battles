using System;
using UnityEngine;

[Serializable]
public class PlayerShip
{
    public string name;
    public Player prefab;
}


public enum GameStates
{
    Menu, Game, GameOver, Debug
}

public class GameManager : MonoSingleton<GameManager>
{
    private GameStates currentGameState;
    public static bool Paused;
    private State[] ListOfStates;
    private static State currentState;
    private float DefaultTimeDeltaScale;

    public int counsEarnInGame;
    public int coinDropInTotal;
    public int score;
    public int MaxLevelUnlocked = 5;
    public int CurrentHeroChoosen;

    [Range(0, 20)]
    public int LevelDifficuilty;

    [SerializeField] private PlayerShip[] players;

    public static int LevelSelected = 0;

    public GameObject levelupAnnouncement;


    public override void Init()
    {
        base.Init();
        new DataController();

    }

    public override void OnQuitGame()
    {
        base.OnQuitGame();

        //DataController.SavePlayerData();
    }

    public static void ShowLevelup()
    {
        GameManager.instance.levelupAnnouncement.SetActive(true);
    }

    private void Start()
    {
        if (LevelDifficuilty > 0)
        {
            SpawnEnemies.SetLevelDifficuilty(LevelDifficuilty);
        }

        new PlayerManager(players);
        MenuState menuState = new MenuState(this);
        MainGameState mainGameState = new MainGameState(this);
        GameOverState gameOverState = new GameOverState(this);

        ListOfStates = new State[]{
            menuState,mainGameState,gameOverState
            };

        SetState(currentGameState);

        PlayerData playerData = DataController.GetPlayerData();
 
        AudioManager.instance.SetMusicVolume(playerData.MusicVolume);

        AudioManager.instance.SetSoundVolume(playerData.SFXVolume);

        DefaultTimeDeltaScale = Time.fixedDeltaTime;


  
    }

    public void PauseTheGame(bool value = true)
    {
        if (value)
        {
            Time.timeScale = 0;
            Time.fixedDeltaTime = 0;
            Paused = true;
        }
        else
        {
            Time.timeScale = 1;
            Time.fixedDeltaTime = DefaultTimeDeltaScale;
            Paused = false;
        }
    }

    public void SetState(GameStates gameState)
    {
        if (currentState != null)
        {
           currentState.OnStateExit();
        }

       currentGameState = gameState;

        currentState = ListOfStates[(int)gameState];

        if (currentState != null)
        {
           currentState.OnStateEnter();
        }
    }

    public GameStates GetCurrentState()
    {
        return currentGameState;
    }
}