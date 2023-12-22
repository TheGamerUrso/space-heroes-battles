using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Analytics;

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
    public Sprite[] sprites;

    public float sceneLoadProgress;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            playerData.EarnXP(100);
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

        DontDestroyOnLoad(gameObject);

        _instancedSystemPrefabs = new List<GameObject>();
        _loadOperation = new List<AsyncOperation>();

        InstantiateSystemPrefabs();

        DefaultTimeDeltaScale = Time.fixedDeltaTime;

        DOTween.Init(autoKillMode, useSafeMode, logBehaviour);

        PersistantData.LoadData();

        PlayerManager pm = new PlayerManager(this, this);

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

        if (SceneManager.GetActiveScene().buildIndex == (int)LevelEnum.boot)
        {
            LoadScene(LevelEnum.Intro, false);
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

    [ContextMenu("Load Menu")]
    public void LoadMainenu()
    {
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


