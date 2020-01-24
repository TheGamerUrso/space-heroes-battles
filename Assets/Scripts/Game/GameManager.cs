using System;
using UnityEngine;
using UnityEngine.EventSystems;

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

public class GameManager : Singleton<GameManager>
{
    public delegate void OnLoadData();
    public event OnLoadData OnLoadDataCompleted;

    public static bool IsGameOver;
    public  static bool Paused;


    private static float DefaultTimeDeltaScale;

    public static int counsEarnInGame;
    public static int coinDropInTotal;
    public static int score;
    public static int MaxLevelUnlocked = 5;
    public static int CurrentHeroChoosen;

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

    public void ShowLevelup()
    {
        Instance.levelupAnnouncement.SetActive(true);
    }

    private void Start()
    {
        if (LevelDifficuilty > 0)
        {
            SpawnEnemies.SetLevelDifficuilty(LevelDifficuilty);
        }
        PlayerData playerData = DataController.GetPlayerData();
        AudioManager.Instance.SetMusicVolume(playerData.MusicVolume);

        AudioManager.Instance.SetSoundVolume(playerData.SFXVolume);

        DefaultTimeDeltaScale = Time.fixedDeltaTime;

        GameEventSystem.OnPlayerLevelUpHandled += ShowLevelup;
    }

    public static void PauseTheGame(bool value = true)
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

    public static bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

 
}