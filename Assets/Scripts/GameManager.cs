using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;
using Doozy.Engine.UI;
using UnityEngine.UI;

[Serializable]
public class PlayerShipElement
{
    public string name;
    public PlayerShip prefab;
}


public class GameManager : Singleton<GameManager>
{
    public Action OnLoadDataCompleted;
    public Action<bool> OnPauseGame;

    public Action<string,bool> OnSceneLoadStart;
    public Action<string,bool> OnSceneLoadFinished;
    public Action<float> OnSceneLoadProgress;

    private static float DefaultTimeDeltaScale;

    [Range(0, 20)]
    public int LevelDifficuilty;

    [SerializeField] private PlayerShipElement[] PlayerShips;

    public static int LevelIndexSelected = 0;
    private PlayerData playerData;

    [SerializeField] private GameObject levelupAnnouncement;
    [SerializeField]private GameObject[] SystemPrefabs;

    private List<GameObject> _instancedSystemPrefabs;

    private bool autoKillMode;
    private bool useSafeMode;
    private LogBehaviour logBehaviour;


    #region SceneManagment
    public bool IsLoading { get; private set; }
    protected List<string> ActiveScenes;

    private string _currentLevelName;
    List<AsyncOperation> _loadOperation;
    private bool unloading;

    private static AsyncOperation loadLvl;

    public bool ManualFadeIn = false;

    private Animator animator;
    public Image progressBar;

    public CanvasGroup BlockRaycast;
    public GameObject ProgressBarPanel;
    public GameObject Content;

    public string currentLevelLoaded;

    public UIView LoadingScreen;

    public GateControl[] Gates;

    #endregion



    public PlayerShipElement[] ListOfPlayerShips()
    {
        return PlayerShips;
    }

    private void OnApplicationQuit()
    {
        UnSubscribeToEvents();
        SaveSystem.SaveGame();
    }

    protected override void OnAwake()
    {
        DontDestroyOnLoad(gameObject);

        animator = GetComponentInChildren<Animator>();

        _instancedSystemPrefabs = new List<GameObject>();
        _loadOperation = new List<AsyncOperation>();
        ActiveScenes = new List<string>();
        InstantiateSystemPrefabs();

        DefaultTimeDeltaScale = Time.fixedDeltaTime;

        DOTween.Init(autoKillMode, useSafeMode, logBehaviour);

        PersistantData.LoadData();

        PlayerManager pm = new PlayerManager(this,this);

        AdvertismentManager.Instance.Initialize();

        pm.LoadPlayerSettings();


        SubscribeToEvents();

        OnLoadDataCompleted?.Invoke();
    }

    public void UnSubscribeToEvents()
    {
        playerData.OnLevelValueChanged -= OnLevelValueChanged;
    }

    public void SubscribeToEvents()
    {
        playerData = PersistantData.GetPlayerData();
        playerData.OnLevelValueChanged += OnLevelValueChanged;
    }

    public void OnLevelValueChanged(int Level)
    {
        Instance.levelupAnnouncement.SetActive(true);

        PlayerShipData playerShipData = playerData.playerShipData[0];
        if (GooglePlayServicesManager.Instance)
            GooglePlayServicesManager.Instance.ReportAchivementProgress(EasyMobile.EM_GameServicesConstants.Achievement_Max_Power, playerShipData.level / 20);

    }

    private void Start()
    {
        if (SceneManager.sceneCount > 1)
        {

        }
        else if (SceneManager.sceneCount <= 1)
        {
            LoadLevel("Intro");
        }

        Hide();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            PlayerData playerData = PersistantData.GetPlayerData();
            playerData.AddCoin(999);
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

    public void ResetLevel()
    {
        StartCoroutine(ShowLoadingScreen(SceneManager.GetActiveScene().name));
    }

    public void LoadScene(string level)
    {
        StartCoroutine(ShowLoadingScreen(level));
    }

    public void LoadMainenu()
    {
        GameSession.ShowAdCounter--;
        if (GameSession.ShowAdCounter <= 0)
        {
            GameSession.ShowAdCounter = 5;
            AdvertismentManager.ShowAdvertisment();
        }
        LoadScene("Main");
    }

    void OnLoadOperationComplete(AsyncOperation ao)
    {
        if (_loadOperation.Contains(ao))
        {
            _loadOperation.Remove(ao);
            //dispatch message
            //transition between scenes
            IsLoading = false;
            //OnSceneLoadFinished?.Invoke();
            UpdateProgress(1f);

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(currentLevelLoaded));

            OnSceneLoadFinished?.Invoke(currentLevelLoaded, ManualFadeIn);
        }

        //Debug.Log("Load Complete.");
    }

    void OnUnloadOperationComplete(AsyncOperation ao)
    {
        // Debug.Log("Unload Complete.");
        unloading = false;

    }
    public void UnloadLevel(string levelName)
    {
        unloading = true;

        AsyncOperation ao = SceneManager.UnloadSceneAsync(levelName);
        if (ao == null)
        {
            Debug.LogError("[SceneController] Unable to unload level" + levelName);
            return;
        }
        ao.completed += OnUnloadOperationComplete;
    }
    protected void UpdateProgress(float progress)
    {
        OnSceneLoadProgress?.Invoke(progress);
    }

    private IEnumerator LoadSceneAsync(string levelName, float delay = 0)
    {
        for (int i = 0; i < ActiveScenes.Count; i++)
        {
            string item = ActiveScenes[i];
            UnloadLevel(item);
        }

        ActiveScenes.Clear();

        WaitForEndOfFrame waitForEndFrame = new WaitForEndOfFrame();

        while (unloading)
        {
            yield return waitForEndFrame;

        }


        AsyncOperation ao = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
        ao.completed += OnLoadOperationComplete;
        _loadOperation.Add(ao);
        _currentLevelName = levelName;
        ActiveScenes.Add(levelName);
        currentLevelLoaded = levelName;

        if (ao == null)
        {
            Debug.LogError("[SceneController] Unable to load level" + levelName);
        }

        System.GC.Collect();

        while (ao.isDone == false)
        {
            UpdateProgress(ao.progress);
            yield return null;
        }

        Hide();
    }
    private IEnumerator ShowLoadingScreen(string level)
    {
        Show();

        WaitForSeconds waitForSec = new WaitForSeconds(2.0f);
        yield return waitForSec;
        StartCoroutine(LoadSceneAsync(level));
    }

    private void Show()
    {
        Content.SetActive(true);
        BlockRaycast.blocksRaycasts = true;
        for (int i = 0; i < Gates.Length; i++)
        {
            GateControl gate = Gates[i];
            gate.CloseGate();
        }
    }

    private void Hide()
    {
        Content.SetActive(false);
        BlockRaycast.blocksRaycasts = false;
        for (int i = 0; i < Gates.Length; i++)
        {
            GateControl gate = Gates[i];
            gate.OpenGate();
        }
    }
    public void LoadLevel(string levelName, float delay = 0)
    {
        if (IsLoading)
        {
            return;
        }
        IsLoading = true;
        OnSceneLoadStart?.Invoke(currentLevelLoaded, ManualFadeIn);
        UpdateProgress(0f);

        StartCoroutine(LoadSceneAsync(levelName, delay));
    }


    public void PauseTheGame(bool value)
    {
        Game.Paused = value;

        OnPauseGame?.Invoke(Game.Paused);

        if (Game.Paused)
        {
            Time.timeScale = 0;
            Time.fixedDeltaTime = 0;
            Game.Paused = true;

        }
        else
        {
            Time.timeScale = 1;
            Time.fixedDeltaTime = DefaultTimeDeltaScale;
            Game.Paused = false;
        }


    }
}