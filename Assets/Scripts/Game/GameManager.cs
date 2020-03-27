using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

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


    public static bool Paused;
    private bool firstRun;

    private static float DefaultTimeDeltaScale;

    public static int counsEarnInGame;
    public static int coinDropInTotal;
    public static int score;
    public static int MaxLevelUnlocked = 5;
    public static int CurrentHeroChoosen;

    [Range(0, 20)]
    public int LevelDifficuilty;

    [SerializeField] private PlayerShipElement[] PlayerShips;

    public static int LevelSelected = 0;

    public GameObject levelupAnnouncement;

    public bool debug;

    public GameObject[] SystemPrefabs;
    private List<GameObject> _instancedSystemPrefabs;

    public bool autoKillMode;
    public bool useSafeMode;
    public LogBehaviour logBehaviour;

    private PlayerManager playerManager;
    private DataController dataController;

    public PlayerShipElement[] ListOfPlayerShips()
    {
        return PlayerShips;
    }

    public static bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }


    protected override void OnAwake()
    {
        Debug.Log("Loading Data");
        new DataController();

        DataController.Setup();

        Debug.Log("Set up Players");
        new PlayerManager();
        PlayerManager.LoadPlayerSettings();

        DefaultTimeDeltaScale = Time.fixedDeltaTime;

        GameEventSystem.PlayerLeveledUp += ShowLevelup;

        GameEventSystem.OnShipSelect += ShipSelected;



    }

    public void ShipSelected(int shipSelected)
    {
        CurrentHeroChoosen = shipSelected;
    }

    protected override void OnCleanup()
    {
        base.OnCleanup();

        //DataController.SavePlayerData();
    }

    public void ShowLevelup()
    {
        Instance.levelupAnnouncement.SetActive(true);
    }

    private void Start()
    {
        DOTween.Init(autoKillMode, useSafeMode, logBehaviour);

        DontDestroyOnLoad(gameObject);
        _instancedSystemPrefabs = new List<GameObject>();
        InstantiateSystemPrefabs();


        OnLoadDataCompleted?.Invoke();

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name.Equals("boot"))
            {
                Debug.Log("boot found skip");
            }

            if (SceneManager.sceneCount <= 1)
            {
                Debug.Log("Continue");
                SceneLoader.Instance.LoadLevel("Intro");
            }   
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
}