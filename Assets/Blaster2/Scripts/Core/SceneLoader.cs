using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TheGamerUrso.Core
{
    /// <summary>
    /// Core director responsible for handling additive scene loading, unloads, 
    /// loading screen overlays, and event notifications.
    /// </summary>
    [DisallowMultipleComponent]
    public class SceneLoader : MonoSingleton<SceneLoader>
    {
        //====================================================================================================
        // Fields & Configuration
        //====================================================================================================
        [Header("Debug Status")]
        [SerializeField] private List<string> activeScenes = new List<string>();

        private readonly List<AsyncOperation> loadOperations = new List<AsyncOperation>();
        private static LevelEnum currentLevelLoad;
        private float totalScreenProgress;

        private IEventService eventService;

        // Cached yield instructions to avoid GC allocations in coroutines
        private readonly WaitForSeconds shortWait = new WaitForSeconds(0.1f);
        private readonly WaitForSeconds longWait = new WaitForSeconds(1.0f);
        
        //====================================================================================================
        // Properties
        //====================================================================================================
        public static string CurrentLevelName { get; private set; }
        
        //====================================================================================================
        // Unity Lifecycle & Initialization
        //====================================================================================================

        protected override void Setup()
        {
            base.Setup();
            eventService = GameContext.Get<IEventService>();

            activeScenes.Clear();
            loadOperations.Clear();

            // Register initial scene state on game boot
            int sceneCount = SceneManager.sceneCount;
            for (int i = 0; i < sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.buildIndex >= 0)
                {
                    activeScenes.Add(((LevelEnum)scene.buildIndex).ToString());
                }
            }
        }
        //====================================================================================================
        // Public API Methods
        //====================================================================================================
        public static Coroutine LoadScene(LevelEnum level, bool showLoading = true)
        {
            if (Instance == null)
            {
                Debug.LogError($"[{nameof(SceneLoader)}] Cannot load scene {level}. Instance is null!");
                return null;
            }

            return Instance.StartCoroutine(Instance.LoadLevelCoroutine((int)level, showLoading));
        }
        //====================================================================================================
        public static void RestartLevel()
        {
            int activeBuildIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(activeBuildIndex);
        }
        //====================================================================================================
        public void UnloadLevel(string levelName)
        {
            if (string.IsNullOrEmpty(levelName)) return;

            AsyncOperation unloadAO = SceneManager.UnloadSceneAsync(levelName);
            if (unloadAO == null)
            {
                Debug.LogError($"[{nameof(SceneLoader)}] Unable to initiate unload for scene: '{levelName}'");
                return;
            }

            loadOperations.Add(unloadAO);
            unloadAO.completed += OnUnloadOperationComplete;
        }
        //====================================================================================================
        // Coroutines & Internal Async Flow
        //====================================================================================================
        private IEnumerator LoadLevelCoroutine(int level, bool showLoading)
        {
            Time.timeScale = 1.0f;

            // Optional Additive Loading Screen
            if (showLoading)
            {
               // AsyncOperation loadingAO = SceneManager.LoadSceneAsync((int)LevelEnum.LOADING, LoadSceneMode.Additive);
               // while (!loadingAO.isDone)
               // {
                   yield return longWait;
               // }
            }

            // Unload previously active scene layers without generating garbage allocations
            if (activeScenes.Count > 0)
            {
                for (int i = activeScenes.Count - 1; i >= 0; i--)
                {
                    UnloadLevel(activeScenes[i]);
                }
                activeScenes.Clear();
            }

            currentLevelLoad = (LevelEnum)level;

            AsyncOperation targetLoadAO = SceneManager.LoadSceneAsync(level, LoadSceneMode.Additive);
            if (targetLoadAO != null)
            {
                targetLoadAO.completed += OnLoadOperationComplete;
                loadOperations.Add(targetLoadAO);
            }

            PublishLoadingProgress(0f);

            yield return StartCoroutine(GetSceneLoadProgress());

            PublishLoadingProgress(1f);

            System.GC.Collect();

            if (showLoading)
            {
                yield return shortWait;
               // SceneManager.UnloadSceneAsync((int)LevelEnum.LOADING);
            }

            CurrentLevelName = ((LevelEnum)level).ToString();
            activeScenes.Add(CurrentLevelName);
        }
        //====================================================================================================
        private IEnumerator GetSceneLoadProgress()
        {
            while (loadOperations.Count > 0)
            {
                totalScreenProgress = 0f;

                for (int i = 0; i < loadOperations.Count; i++)
                {
                    // Unity AsyncOperation progress caps at 0.9 before activation
                    float normalizedProgress = Mathf.Clamp01(loadOperations[i].progress / 0.9f);
                    totalScreenProgress += normalizedProgress;
                }

                float aggregateProgress = totalScreenProgress / loadOperations.Count;
                PublishLoadingProgress(aggregateProgress);

                yield return null;
            }
        }
        //====================================================================================================
        // Event Callbacks
        //====================================================================================================
        private void OnLoadOperationComplete(AsyncOperation ao)
        {
            if (loadOperations.Contains(ao))
            {
                loadOperations.Remove(ao);
                Scene targetScene = SceneManager.GetSceneByName(currentLevelLoad.ToString());
                if (targetScene.IsValid())
                {
                    SceneManager.SetActiveScene(targetScene);
                }

                UpdateProgress(1f);

                SceneManager.SetActiveScene(SceneManager.GetSceneByName(CurrentLevelName));

                Events.OnSceneLoadFinished?.Invoke(CurrentLevelName);

            }
        }
        //====================================================================================================
        public void OnUnloadOperationComplete(AsyncOperation ao)
        {
            loadOperations.Remove(ao);
        }
        //====================================================================================================
        // Helpers
        //====================================================================================================
        private void PublishLoadingProgress(float progressNormalized)
        {
            if (eventService == null) return;

            //LoadingProgressEvent progressEvent = new LoadingProgressEvent(progressNormalized);
            //eventService.Publish(progressEvent);
        }
        //====================================================================================================
        protected void UpdateProgress(float progress)
        {
            Events.OnSceneLoadProgress?.Invoke(progress);
            //GameManager.Instance.sceneLoadProgress = progress;
        }
    }
}
