using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TheGamerUrso.SceneLoader;
using System.Collections;

[Serializable]
public class PlayerShipElement
{
    public string name;
    public PlayerShip prefab;
}


public class GameManager : Singleton<GameManager>
{
    public delegate void OnLoadData();
    public event OnLoadData OnLoadDataCompleted;

    public delegate void PauseGame(bool value);
    public PauseGame OnPauseGame;


    public static bool Paused;
    private bool firstRun;

    private static float DefaultTimeDeltaScale;

    public static int counsEarnInGame;
    public static int coinDropInTotal;
    public static int score;
    public static int MaxLevelUnlocked = 5;


    [Range(0, 20)]
    public int LevelDifficuilty;

    [SerializeField] private PlayerShipElement[] PlayerShips;

    public static int LevelIndexSelected = 0;
    private PlayerData playerData;

    public GameObject levelupAnnouncement;

    public bool debug;

    public GameObject[] SystemPrefabs;
    private List<GameObject> _instancedSystemPrefabs;

    public bool autoKillMode;
    public bool useSafeMode;
    public LogBehaviour logBehaviour;

    public PlayerShipElement[] ListOfPlayerShips()
    {
        return PlayerShips;
    }

    private void OnApplicationQuit()
    {
        playerData.OnLevelValueChanged -= OnLevelValueChanged;
        SaveSystem.SaveGame();
    }

    protected override void OnAwake()
    {
        Debug.Log("Loading Data"); 
        PersistantData.LoadData();

        Debug.Log("Set up Players");
        PlayerManager pm = new PlayerManager(this, GameManager.Instance);
        pm.LoadPlayerSettings();

        DefaultTimeDeltaScale = Time.fixedDeltaTime;

        playerData = PersistantData.GetPlayerData();

        playerData.OnLevelValueChanged += OnLevelValueChanged;

        DOTween.Init(autoKillMode, useSafeMode, logBehaviour);

        DontDestroyOnLoad(gameObject);

        _instancedSystemPrefabs = new List<GameObject>();

        InstantiateSystemPrefabs();


        OnLoadDataCompleted?.Invoke();

    }

    protected override void OnCleanup()
    {
        base.OnCleanup();
    }

    public void OnLevelValueChanged(int Level)
    {
        Instance.levelupAnnouncement.SetActive(true);
    }

    private void Start()
    {
        if (SceneManager.sceneCount > 1)
        {
            Debug.Log("boot found skip");
        }
        else if (SceneManager.sceneCount <= 1)
        {
            Debug.Log("Continue");
            SceneLoader.Instance.LoadLevel("Intro");
        }
    }

    private void InstantiateSystemPrefabs()
    {
        foreach (var systemPrefab in SystemPrefabs)
        {
            var prefabInstance = Instantiate(systemPrefab);
            _instancedSystemPrefabs.Add(prefabInstance);
        }
    }

    public void PauseTheGame(bool value)
    {
        Paused = value;

        OnPauseGame?.Invoke(Paused);

        if (Paused)
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
}