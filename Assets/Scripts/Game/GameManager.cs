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

    public static int LevelSelected = 0;

    public GameObject levelupAnnouncement;

    public bool debug;

    public GameObject[] SystemPrefabs;
    private List<GameObject> _instancedSystemPrefabs;

    public bool autoKillMode;
    public bool useSafeMode;
    public LogBehaviour logBehaviour;


    public PlayerData playerData;
    private GameSettings gameSettings;


    private static MissionCollection missionCollection;
    private static LevelObjectiveCollection LevelObjectiveCollection;

    public PlayerShipElement[] ListOfPlayerShips()
    {
        return PlayerShips;
    }

    private void OnApplicationQuit()
    {
        SaveSystem.SaveGame();
    }

    protected override void OnAwake()
    {
        Debug.Log("Loading Data");
        Setup(PlayerShips.Length);

        Debug.Log("Set up Players");
        PlayerManager pm = new PlayerManager(this, GameManager.Instance);
        pm.LoadPlayerSettings();

        DefaultTimeDeltaScale = Time.fixedDeltaTime;

        GameEventSystem.PlayerLeveledUp += ShowLevelup;

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

    public void ShowLevelup()
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

        OnPauseGame?.Invoke(Paused);
    }

    public void Setup(int playerShips = 3)
    {
        missionCollection = JsonSystem.LoadMissions();
        LevelObjectiveCollection = JsonSystem.LoadLevelObjectiveData();
        playerData = new PlayerData(playerShips);

        int firstRunIndex = 0;

        if (PlayerPrefs.HasKey("FirstRun"))
        {
            firstRunIndex = PlayerPrefs.GetInt("FirstRun");
        }

        if (firstRunIndex == 1)
        {
            SaveSystem.LoadGame();

            GameSettings.Initialize(
                playerData.SFXVolume,
                playerData.MusicVolume,
                playerData.AutoAttack,
                playerData.mute,
                playerData.distance);

            Dictionary<string, LevelObjectiveData[]> Challanges = playerData.GetListOfObjectives();
            int missionsCompleted = 0;
            foreach (KeyValuePair<string, LevelObjectiveData[]> item in Challanges)
            {
                if (item.Value[0].completed == true)
                {
                    missionsCompleted++;
                }
            }

            playerData.LevelUnlocked = missionsCompleted;
        }
        else if (firstRunIndex == 0)
        {
            PlayerPrefs.SetInt("FirstRun", 1);
            SaveSystem.SaveGame();
        }

        GenerateLevelObjectiveData();
    }

    public Dictionary<string, LevelObjectiveData[]> GetListOfLevelChallanges()
    {
        return playerData.ListOfLevelChallenges;
    }
    public LevelObjectiveData[] GetLevelChallegeById(string levelId)
    {
        return GetLevelObjectivesByID(levelId);
    }

    public LevelObjectiveData[] GetLevelObjectivesByID(string levelId)
    {
        LevelObjectiveData[] objectives;
        if (playerData.ListOfLevelChallenges.TryGetValue(levelId, out objectives))
        {
            return objectives;
        }

        return null;
    }
    public void SetPlayerData(PlayerData playerData)
    {
        this.playerData = playerData;
    }

    public PlayerData GetPlayerData()
    {
        return playerData;
    }


    public Mission GetMission(int index)
    {
        return missionCollection.GetMission(index);
    }

    public MissionCollection GetMissionCollection()
    {
        return missionCollection;
    }

    public int GetNumberOfData()
    {
        return GetListOfLevelChallanges().Count;
    }

    public void GenerateLevelObjectiveData()
    {
        if (playerData.ListOfLevelChallenges.Count == 0)
        {
            for (int i = 0; i < LevelObjectiveCollection.LevelObjective.Levels.Length - 1; i++)
            {
                int size = LevelObjectiveCollection.LevelObjective.Levels[i].Objectives.Length;
                LevelObjectiveData[] objectiveListData = new LevelObjectiveData[size];
                for (int x = 0; x < LevelObjectiveCollection.LevelObjective.Levels[i].Objectives.Length; x++)
                {
                    objectiveListData[x] = new LevelObjectiveData(
                        LevelObjectiveCollection.LevelObjective.Levels[i].Objectives[x].ID, LevelObjectiveCollection.LevelObjective.Levels[i].Objectives[x].Description);
                }

                playerData.AddToListLevelChallenges(LevelObjectiveCollection.LevelObjective.Levels[i].ID, objectiveListData);
            }
        }
    }
}