using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using EasyMobile;

[Serializable]
public struct PlayerShipElement
{
    public string name;
    public GameObject prefab;
}

public class GameManager : MonoSingleton<GameManager>
{

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
    private static GameStateEnum _currentGameState;
    public static GameStateEnum CurrentGameState
    {
        get
        {
            return _currentGameState;
        }

        private set
        {
            _currentGameState = value;
        }
    }
    #endregion

    public string currentLevelLoaded;
    private Level currentLevelSelected;
    public Sprite[] sprites;
    private MissionCollection missionCollection;

    public float sceneLoadProgress;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            playerData.EarnXP(100);
        }
    }

    public Level GetCurrentLevelSelected()
    {
        if (currentLevelSelected == null)
        {
            currentLevelSelected = PersistantData.GetLevels()[1];
        }
        return currentLevelSelected;
    }

    public string GetStory(int missionIndex)
    {
        currentLevelSelected = PersistantData.GetMission(missionIndex - 1);
        return currentLevelSelected.mission.Description;
    }

    public MissionCollection GetMissions()
    {
        return missionCollection;
    }

    public Level GetCurrentMission()
    {
        return currentLevelSelected;
    }

    public Level GetMission(LevelEnum level)
    {
        return PersistantData.GetLevels()[(int)level - (int)LevelEnum.Level0];
    }

    public void SetMission(Level level)
    {
        currentLevelSelected = level;

        if (level.mission.ID > 0)
        {
            //StoryController.Instance.ShowStory(level);
        }
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

        if (!RuntimeManager.IsInitialized())
            RuntimeManager.Init();

        DontDestroyOnLoad(gameObject);

        _instancedSystemPrefabs = new List<GameObject>();
        _loadOperation = new List<AsyncOperation>();

        InstantiateSystemPrefabs();

        DefaultTimeDeltaScale = Time.fixedDeltaTime;

        DOTween.Init(autoKillMode, useSafeMode, logBehaviour);

        PersistantData.LoadData();

        PlayerManager pm = new PlayerManager(this, this);

        AdvertismentManager.Initialize();

        new AchievementSystem(PersistantData.Instance.Achievements.ListOfAchievelemtnts);

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
        Instantiate(LevelUpPrefab);
    }

    private void Start()
    {
        Hide();

        playerData = PersistantData.GetPlayerData();
        Events.OnLevelValueChanged += OnLevelValueChanged;

        playerData = PersistantData.GetPlayerData();
        playerShipData = playerData.GetCurrentPlayerShipData();

        missionCollection = PersistantData.GetMissionCollection();

        if (SceneManager.GetActiveScene().buildIndex == (int)LevelEnum.boot)
        {
            LoadScene(LevelEnum.Intro, false);
        }
        if (GPServices.IsInitialized())
        {
            GPServices.LoadLocalUserScore();
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

    [ContextMenu("Reset Level")]
    public void ResetLevel()
    {
        StartCoroutine(ResetSceneAsync((LevelEnum)SceneManager.GetActiveScene().buildIndex));
    }

    public void LoadScene(LevelEnum level, bool showLoadingScreen = true)
    {
        StartCoroutine(ShowLoadingScreen(level, showLoadingScreen));
    }
    [ContextMenu("Next Level")]
    public void NextLevel()
    {
        Scene scene = SceneManager.GetActiveScene();
        int nextLevelID = (scene.buildIndex + 1) - (int)LevelEnum.Level0;

        if (nextLevelID > 9)
        {
            nextLevelID = 9;
        }

        Level nextLevel = PersistantData.GetLevels()[nextLevelID];

        GameManager.Instance.SetMission(nextLevel);
        GameManager.Instance.LoadScene((LevelEnum)nextLevel.mission.ID);
    }
    [ContextMenu("Load Menu")]
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

    private IEnumerator ResetSceneAsync(LevelEnum levelName)
    {
        string activeScene = SceneManager.GetActiveScene().name;

        if (activeScene.Equals(levelName.ToString()))
        {
            SceneManager.LoadSceneAsync((int)levelName, LoadSceneMode.Single);

        }
        else
        {
            ao = SceneManager.LoadSceneAsync((int)levelName);

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

            System.GC.Collect();

            Hide();

            yield return new WaitForSeconds(2.0f);
            LoadingScreen.SetActive(false);
        }
    }

    private IEnumerator LoadSceneAsync(LevelEnum levelName)
    {
        ao = SceneManager.LoadSceneAsync((int)levelName);

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

        System.GC.Collect();

        Hide();

        yield return new WaitForSeconds(2.0f);
        LoadingScreen.SetActive(false);
    }

    private IEnumerator ShowLoadingScreen(LevelEnum level, bool showLoadingScreen = true)
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
        playerData.SetScore(0, ammount);
        GPServices.ReportLeaderboards(ammount);
    }

    public void PlayerChallengesCheck()
    {
        Scene scene = SceneManager.GetActiveScene();
        Level level = GetMission((LevelEnum)scene.buildIndex);

        var killed = (Game.EnemySpawnInTotal - Game.EnemyKilled) / Game.EnemySpawnInTotal;

        var levelObjectiveDatas = level.objectiveListData;

        if (levelObjectiveDatas[0].completed == false)
        {
            levelObjectiveDatas[0].completed = true;
            playerData.EarnXP(20 * playerData.GetCurrentPlayerShipData().level);
        }

        if (!levelObjectiveDatas[1].completed && killed < .8f)
        {
            levelObjectiveDatas[1].completed = true;
            playerData.EarnXP(20 * playerData.GetCurrentPlayerShipData().level);
        }

        if (!levelObjectiveDatas[2].completed && killed < .4f)
        {
            levelObjectiveDatas[2].completed = true;
            playerData.EarnXP(40 * playerData.GetCurrentPlayerShipData().level);
        }
        if (!levelObjectiveDatas[3].completed && killed < .2f)
        {
            levelObjectiveDatas[3].completed = true;
            playerData.EarnXP(40 * playerData.GetCurrentPlayerShipData().level);
        }

        if (!levelObjectiveDatas[4].completed && playerData.PlayedGame && !playerData.GotHitInGame)
        {
            levelObjectiveDatas[4].completed = true;
            playerData.EarnXP(80 * playerData.GetCurrentPlayerShipData().level);
        }

        if (levelObjectiveDatas.Length > 5)
        {
            if (!levelObjectiveDatas[5].completed)
            {
                levelObjectiveDatas[5].completed = true;
                playerData.EarnXP(100 * playerData.GetCurrentPlayerShipData().level);
            }
        }


        int levelIndex = level.mission.ID - (int)LevelEnum.Level0;
        int AchievementIndex = level.mission.ID - (int)LevelEnum.Level1;

        playerData.LevelUnlocked = levelIndex + 1;

        AchievementSystem.instance.Report(AchievementIndex, 1);

        if (playerData.LevelUnlocked > 9)
        {
            playerData.SetSurvivalUnlockedLock(true);
        }


    }

    [ContextMenu("Finish Quests")]
    public void FInishQuests()
    {
        for (int i = 0; i < playerData.ListOfOnGoingObjectives.Count; i++)
        {
            ObjectiveData objective = playerData.ListOfOnGoingObjectives[i];
            objective.UpdateProgress(objective.requirment);
        }
    }

    public void PlayerQuestProgress(ObjectiveTypeEnum type, int progress)
    {

        ObjectiveData objectiveData = playerData.GetOnGoingObjectiveById(type);

        if (type == ObjectiveTypeEnum.SPEND)
        {
            objectiveData.UpdateProgress(progress);
        }
        else
        {
            Scene scene = SceneManager.GetActiveScene();
            Level level = GetMission((LevelEnum)scene.buildIndex);
            int levelIndex = level.mission.ID - (int)LevelEnum.Level0;
            if (objectiveData != null)
            {
                if (type == ObjectiveTypeEnum.SURVIVE)
                {
                    if (objectiveData.requirment.Equals(levelIndex))
                    {
                        objectiveData.UpdateProgress(levelIndex);
                    }
                    return;
                }

                objectiveData.UpdateProgress(progress);
            }
        }
    }

    public void UpdatePlayerStatistics()
    {
        Scene scene = SceneManager.GetActiveScene();
        Level level = GetMission((LevelEnum)scene.buildIndex);
        int levelIndex = level.mission.ID - (int)LevelEnum.Level0;

        playerData.SetScore(levelIndex, Game.Score);
        playerData.PlayedGame = true;
        playerData.Coins += Game.CoinPicked;
        playerData.Kills += Game.EnemyKilled;

        PlayerQuestProgress(ObjectiveTypeEnum.SURVIVE, levelIndex);

        if (!Game.PlayerGotHit)
        {
            Instance.PlayerQuestProgress(ObjectiveTypeEnum.UNHARMED, 0);
        }

        playerShipData.Upgrades[(int)UpgradeTypeEnum.Shield] = 0;

        AchievementSystem.instance.Report(10, Game.EnemyKilled);
        AchievementSystem.instance.Report(11, Game.EnemyKilled);

        SaveSystem.SaveGame();
    }

    [ContextMenu("Unlimited Money")]
    public void AddCoin()
    {
        playerData.AddCoin(99999999);
    }


    [ContextMenu("Level Up")]
    public void LevelUp()
    {
        playerData.EarnXP(playerShipData.xpToLevel - playerShipData.xp);
    }
}


