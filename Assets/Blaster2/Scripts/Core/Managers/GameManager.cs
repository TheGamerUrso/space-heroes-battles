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

namespace TheGamerUrso.Core
{
    public enum GameStateEnum
    {
        PRELOAD, GAME, GAMEOVER, QUITING
    }


    /// <summary>
    /// Core application state director. Manages high-level game state transitions, 
    /// global pause logic, time scaling, and cursor locking.
    /// </summary>
    [DisallowMultipleComponent]
    public class GameManager : ServiceComponent<IAppService>, IAppService
    {
        private static float DefaultTimeDeltaScale;

        private PlayerData playerData;
        private PlayerShipData playerShipData;

        [SerializeField] private GameObject LevelUpPrefab;
        [SerializeField] private PlayerShipElement[] PlayerShips;

        [SerializeField] private GameObject[] SystemPrefabs;

        private List<GameObject> _instancedSystemPrefabs;



        #region Tweening
        private bool autoKillMode;
        private bool useSafeMode;
        private LogBehaviour logBehaviour;
        #endregion Tweening

        #region Properties 
        public GameStateEnum CurrentGameState { get; private set; } = GameStateEnum.PRELOAD;
        
        #endregion

        public string currentLevelLoaded;
        public Sprite[] sprites;

        public float sceneLoadProgress;
        public bool IsPaused { get; set; }



        private IDataService dataService;

        // private ILevelingService levelingService;


        public PlayerShipElement[] ListOfPlayerShips()
        {
            return PlayerShips;
        }

        private void OnApplicationQuit()
        {

            SaveSystem.SaveGame();
        }

    

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);

            _instancedSystemPrefabs = new List<GameObject>();

            InstantiateSystemPrefabs();

            DefaultTimeDeltaScale = Time.fixedDeltaTime;

            DOTween.Init(autoKillMode, useSafeMode, logBehaviour);

            //PlayerManager pm = new PlayerManager(this, this);
            // pm.LoadPlayerSettings();


            dataService = GameContext.Get<IDataService>();
            dataService.Load();
        }

        private void Start()
        {
            //Hide();

            playerData = dataService.GetPlayerData();
            playerShipData = playerData.GetCurrentPlayerShipData();

            // Events.OnLevelValueChanged += OnLevelValueChanged;
            if (SceneManager.GetActiveScene().buildIndex == (int)LevelEnum.boot)
            {
                LoadScene(LevelEnum.Intro, false);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            // Events.OnLevelValueChanged -= OnLevelValueChanged;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                playerData.EarnXP(100);
            }
        }

        public void SetGameState(GameStateEnum nextGameState)
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
            SceneLoader.LoadScene((LevelEnum)SceneManager.GetActiveScene().buildIndex);
        }

        public void LoadScene(LevelEnum level, bool showLoadingScreen = true)
        {
            //StartCoroutine(ShowLoadingScreen(level, showLoadingScreen));
        }

        [ContextMenu("Load Menu")]
        public void LoadMainMenu()
        {
            SceneLoader.LoadScene(LevelEnum.Main);
        }

        public void PauseTheGame(bool value)
        {
            IsPaused = value;

            Events.OnPauseGame?.Invoke(IsPaused);

            if (IsPaused)
            {
                Time.timeScale = 0;
                Time.fixedDeltaTime = 0;
                IsPaused = true;

            }
            else
            {
                Time.timeScale = 1;
                Time.fixedDeltaTime = DefaultTimeDeltaScale;
                IsPaused = false;
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
}


