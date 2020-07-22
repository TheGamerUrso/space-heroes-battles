using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;
using Doozy.Engine.UI;
using UnityEngine.UI;
using EasyMobile;
using System.Linq;

[Serializable]
public struct PlayerShipElement
{
    public string name;
    public GameObject prefab;
}

public class GameManager : MonoSingleton<GameManager>
{
    public enum AchievementType { LEVEL, KILL, UNLOCKHERO }

    private GameStateEnum currentGameState = GameStateEnum.PRELOAD;
    private static float DefaultTimeDeltaScale;
    WaitForSeconds shortWait = new WaitForSeconds(2.0f);



    private PlayerData playerData;
    private PlayerShipData playerShipData;

    [SerializeField] private GameObject LevelUpPrefab;
    [SerializeField] private PlayerShipElement[] PlayerShips;

    [SerializeField] private GameObject[] SystemPrefabs;

    private List<GameObject> _instancedSystemPrefabs;

    #region SceneManagment
    private AsyncOperation ao;
    private List<AsyncOperation> _loadOperation;
    private bool unloading;
    #endregion

    #region Loading Screen
    [Header("LoadingScreen")]
    [SerializeField] private GameObject LoadingScreen;
    [SerializeField] private Image progressBar;

    [SerializeField] private CanvasGroup BlockRaycast;
    [SerializeField] private GameObject ProgressBarPanel;
    [SerializeField] private GameObject Content;
    [SerializeField] private GateControl[] Gates;
    #endregion

    #region Tweening
    private bool autoKillMode;
    private bool useSafeMode;
    private LogBehaviour logBehaviour;
    #endregion Tweening

    #region Properties 
    public static GameStateEnum CurrentGameState
    {
        get
        {
            return CurrentGameState;
        }

        private set
        {
            CurrentGameState = value;
        }
    }
    #endregion

    public static int LevelIndexSelected = 0;
    public string currentLevelLoaded;
    private Mission currentMission;
    private Dictionary<string, LevelObjectiveData[]> Challanges;
    public Sprite[] sprites;
    private MissionCollection missionCollection;

    public float sceneLoadProgress;

    public string GetStory(int missionIndex)
    {
        currentMission = PersistantData.GetMission(missionIndex - 1);
        return currentMission.Description;
    }
    public MissionCollection GetMissions()
    {
        return missionCollection;
    }

    public Mission GetCurrentMission()
    {
        return currentMission;
    }

    public void SetMission(Mission mission)
    {
        currentMission = mission;
        LevelIndexSelected = mission.ID;
        if (LevelIndexSelected > 0)
        {
            StoryController.Instance.ShowStory(GameManager.LevelIndexSelected);
        }
    }

    public LevelObjectiveData[] GetChallenges()
    {
        
        return Challanges[((LevelEnum)currentMission.ID).ToString()];
    }

    public PlayerShipElement[] ListOfPlayerShips()
    {
        return PlayerShips;
    }

    private void OnApplicationQuit()
    {

        SaveSystem.SaveGame();
    }

    protected override void OnCleanup()
    {
        base.OnCleanup();
        Events.OnLevelValueChanged -= OnLevelValueChanged;
    }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);

        _instancedSystemPrefabs = new List<GameObject>();
        _loadOperation = new List<AsyncOperation>();

        InstantiateSystemPrefabs();

        DefaultTimeDeltaScale = Time.fixedDeltaTime;

        DOTween.Init(autoKillMode, useSafeMode, logBehaviour);

        PersistantData.LoadData();

        PlayerManager pm = new PlayerManager(this, this);

        new AdvertismentManager();
        AdvertismentManager.Initialize();

        new GooglePlayServicesManager();
        GooglePlayServicesManager.Initialize();

        pm.LoadPlayerSettings();
    }

    public void ChangeGameState(GameStateEnum nextGameState)
    {
        var previosuGameState = CurrentGameState;

        CurrentGameState = nextGameState;

        if (nextGameState == GameStateEnum.GAME)
        {
            Events.OnLoadDataCompleted?.Invoke();
        }
    }

    public void OnLevelValueChanged(int Level)
    {
        var lvlUp = Instantiate(LevelUpPrefab, transform, false);

        PlayerShipData playerShipData = playerData.playerShipData[0];

        if (GooglePlayServicesManager.GetInitialized())
            GooglePlayServicesManager.ReportAchivementProgress(EasyMobile.EM_GameServicesConstants.Achievement_Max_Power, playerShipData.level / 20);

    }

    private void Start()
    {
        Hide();

        playerData = PersistantData.GetPlayerData();
        Events.OnLevelValueChanged += OnLevelValueChanged;

        if (Challanges == null)
            Challanges = playerData.GetListOfObjectives();

        int missionsCompleted = 1;

        foreach (KeyValuePair<string, LevelObjectiveData[]> item in Challanges)
        {
            if (item.Value[0].completed == true)
            {
                missionsCompleted++;
            }
        }

        playerData = PersistantData.GetPlayerData();
        playerShipData = playerData.GetCurrentPlayerShipData();

        missionCollection = PersistantData.GetMissionCollection();

        playerData.LevelUnlocked = missionsCompleted;

        Challanges = playerData.GetListOfObjectives();

        if (SceneManager.GetActiveScene().buildIndex == (int)LevelEnum.boot)
        {
            LoadScene(LevelEnum.Intro,false);
        }
    }

    private void InstantiateSystemPrefabs()
    {
        foreach (var systemPrefab in SystemPrefabs)
        {
            var prefabInstance = Instantiate(systemPrefab, transform, false);
            prefabInstance.name = systemPrefab.name;
            _instancedSystemPrefabs.Add(prefabInstance);
        }
    }

    public void ResetLevel()
    {
        StartCoroutine(ShowLoadingScreen((LevelEnum)SceneManager.GetActiveScene().buildIndex));
    }

    public void LoadScene(LevelEnum level,bool showLoadingScreen = true)
    {
        StartCoroutine(ShowLoadingScreen(level, showLoadingScreen));
    }

    public void LoadMainenu()
    {
        Game.ShowAdCounter--;
        if (Game.ShowAdCounter <= 0)
        {
            Game.ShowAdCounter = 5;
            AdvertismentManager.ShowAdvertisment();
        }

        LoadScene(LevelEnum.Main);
    }

    void OnLoadOperationComplete(AsyncOperation ao)
    {
        if (_loadOperation.Contains(ao))
        {
            _loadOperation.Remove(ao);

            UpdateProgress(1f);

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(currentLevelLoaded));

            Events.OnSceneLoadFinished?.Invoke(currentLevelLoaded);
        }

    }

    void OnUnloadOperationComplete(AsyncOperation ao)
    {
        unloading = false;
    }

    public void UnloadLevel(string levelName)
    {
        unloading = true;

        ao = SceneManager.UnloadSceneAsync(levelName);
        if (ao == null)
        {
            Debug.LogError("[SceneController] Unable to unload level:" + levelName);
            return;
        }
        ao.completed += OnUnloadOperationComplete;
    }
    protected void UpdateProgress(float progress)
    {
        Events.OnSceneLoadProgress?.Invoke(progress);
        GameManager.Instance.sceneLoadProgress = progress;
    }

    private IEnumerator LoadSceneAsync(LevelEnum levelName)
    {
        string activeScene = SceneManager.GetActiveScene().name;

        if (activeScene.Equals(levelName.ToString()))
        {
            SceneManager.LoadSceneAsync((int)levelName, LoadSceneMode.Single);

        }
        else
        {
            ao = SceneManager.LoadSceneAsync((int)levelName, LoadSceneMode.Additive);

            ao.completed += OnLoadOperationComplete;
            _loadOperation.Add(ao);
            currentLevelLoaded = levelName.ToString();

            if (ao == null)
            {
                Debug.LogError("[SceneController] Unable to load level" + levelName);
            }

            while (ao.isDone == false)
            {
                UpdateProgress(ao.progress);
                yield return null;
            }

            UnloadLevel(activeScene);

            if (unloading)
            {
                yield return new WaitUntil(() => !unloading);
            }


            System.GC.Collect();

            Hide();

            yield return new WaitForSeconds(2.0f);
            LoadingScreen.SetActive(false);
        }

     
    }
    private IEnumerator ShowLoadingScreen(LevelEnum level,bool showLoadingScreen = true)
    {
        if (showLoadingScreen)
        {
            Show();
        }
        yield return shortWait;
        StartCoroutine(LoadSceneAsync(level));
    }

    private void Show()
    {
        LoadingScreen.SetActive(true);
        Content.SetActive(true);
        //BlockRaycast.blocksRaycasts = true;
        for (int i = 0; i < Gates.Length; i++)
        {
            GateControl gate = Gates[i];
            gate.CloseGate();
        }
    }

    private void Hide()
    {
        Content.SetActive(false);
        //BlockRaycast.blocksRaycasts = false;
        for (int i = 0; i < Gates.Length; i++)
        {
            GateControl gate = Gates[i];
            gate.OpenGate();
        }
    }

    public void PauseTheGame(bool value)
    {
        Game.IsPaused = value;

        Events.OnPauseGame?.Invoke(Game.IsPaused);

        if (Game.IsPaused)
        {
            Time.timeScale = 0;
            Time.fixedDeltaTime = 0;
            Game.IsPaused = true;

        }
        else
        {
            Time.timeScale = 1;
            Time.fixedDeltaTime = DefaultTimeDeltaScale;
            Game.IsPaused = false;
        }
    }

    public void SetSurvivalScore(int ammount)
    {
        int levelIndex = GameManager.LevelIndexSelected;
        playerData.SetScore(levelIndex, Game.Score);
    }

    public void PlayerChallengesCheck()
    {
        int levelIndex = LevelIndexSelected;
        var levelName = "Level" + levelIndex;
        var killed = Game.EnemySpawnInTotal * .9f;
        var collected = Game.EnemySpawnInTotal * .9f;

        var levelObjectiveDatas = playerData.GetLevelObjectives(levelName);

        if (levelObjectiveDatas[0].completed == false)
        {
            levelObjectiveDatas[0].completed = true;
            playerData.EarnXP(20 * playerData.GetCurrentPlayerShipData().level);
        }

        float enemyKilled = Game.EnemyKilled;

        if (!levelObjectiveDatas[1].completed && enemyKilled >= killed)
        {
            levelObjectiveDatas[1].completed = true;
            playerData.EarnXP(30 * playerData.GetCurrentPlayerShipData().level);
        }

        if (!levelObjectiveDatas[2].completed && playerData.PlayedGame && !playerData.GotHitInGame)
        {
            levelObjectiveDatas[2].completed = true;
            playerData.EarnXP(40 * playerData.GetCurrentPlayerShipData().level);
        }

        float coinEarnInGame = Game.CoinPicked;

        if (!levelObjectiveDatas[3].completed && coinEarnInGame >= 0 && coinEarnInGame >= collected)
        {
            levelObjectiveDatas[3].completed = true;
            playerData.EarnXP(10 * playerData.GetCurrentPlayerShipData().level);
        }
    }

    public void UnlockNextMission()
    {
        Dictionary<string, LevelObjectiveData[]> Challanges = playerData.GetListOfObjectives();
        int missionsCompleted = 1;
        foreach (KeyValuePair<string, LevelObjectiveData[]> item in Challanges)
        {
            if (item.Value[0].completed == true)
            {
                Notification notification = new Notification();
                notification.Description = "New Mission Available";
                notification.Name = "Mission Unlocked";
                NotificationSystem.Instance.Add(notification);
                missionsCompleted++;
            }
        }

        int levelIndex = GameManager.LevelIndexSelected;
        PostAchievementProgress(AchievementType.LEVEL, levelIndex);

        if (missionsCompleted > 9)
        {
            playerData.SetSurvivalUnlockedLock(true);
        }

        playerData.LevelUnlocked = missionsCompleted;
    }

    public void PlayerQuestCheck()
    {
        if (playerData == null)
            playerData = PersistantData.GetPlayerData();

        for (int i = 0; i < playerData.ListOfOnGoingObjectives.Count; i++)
        {
            ObjectiveData objective = playerData.ListOfOnGoingObjectives[i];
            switch ((ObjectiveType)objective.objectiveType)
            {
                //case ObjectiveType.Use:
                //    if (objective.completed == false)
                //    {
                //        objective.UpdateProgress(playerData.superUsed);
                //    }
                //    break;
                case ObjectiveType.Unharmed:
                    if (objective.completed == false)
                    {
                        if (playerData.GotHitInGame == false)
                        {
                            ObjectiveData objectiveData = playerData.GetOnGoingObjectiveById(ObjectiveType.Unharmed);
                            objectiveData.UpdateProgress(1);
                        }
                    }
                    break;
                case ObjectiveType.survive:
                    var surviveProgress = objective.progress;
                    surviveProgress++;
                    objective.UpdateProgress(surviveProgress);
                    break;
            }
        }
    }

    public void UpdatePlayerStatistics()
    {
        int levelIndex = GameManager.LevelIndexSelected;
        var levelName = "Level" + levelIndex;
        var killed = Game.EnemySpawnInTotal * .9f;
        var collected = Game.EnemySpawnInTotal * .9f;

        playerData.SetScore(levelIndex, Game.Score);
        playerData.PlayedGame = true;
        playerData.Coins += Game.CoinPicked;
        playerData.Kills += Game.EnemyKilled;

        playerShipData.Upgrades[(int)UpgradeTypeEnum.Shield] = 0;


        SaveSystem.SaveGame();
    }

    public void PostAchievementProgress(AchievementType achievement, int progress)
    {
#if UNITY_ANDROID
        if (GooglePlayServicesManager.GetInitialized())
        {
            switch (achievement)
            {
                case AchievementType.LEVEL:
                    GooglePlayServicesManager.UnlockAchievement(progress);

                    break;
                case AchievementType.KILL:
                    GooglePlayServicesManager.ReportAchivementProgress(EM_GameServicesConstants.Achievement_Piece_of_Cake, progress);
                    GooglePlayServicesManager.ReportAchivementProgress(EM_GameServicesConstants.Achievement_Destroyer, progress);
                    break;
                case AchievementType.UNLOCKHERO:
                    break;
                default:
                    break;
            }
        }
#elif UNITY_EDITOR
     Debug.Log("Unlocked" + achievement.ToString()); 
#endif
    }

}


