using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public delegate void SceneLoadEvent(string currentLevelLoaded, bool manualFadeIn);
public delegate void SceneLoadProgressEvent(float progress);
namespace TheGamerUrso
{
    namespace SceneLoader
    {
        public class SceneLoader : Singleton<SceneLoader>
        {
            public SceneLoadEvent OnSceneLoadStart;
            public SceneLoadEvent OnSceneLoadFinished;
            public SceneLoadProgressEvent OnSceneLoadProgress;
            public bool IsLoading { get; private set; }
            protected List<string> ActiveScenes;

            private string _currentLevelName;
            List<AsyncOperation> _loadOperation;
            private bool unloading;

            private static AsyncOperation loadLvl;

            public bool ManualFadeIn = false;

            private Animator animator;
            public Image progressBar;

            public Image BlockRaycast;
            public GameObject ProgressBarPanel;
            public GameObject Content;

            public string currentLevelLoaded;
            public AudioClip appearSFX;
            public AudioClip disapearSFX;
            public AudioSource audioSouce;
            protected override void OnAwake()
            {
                base.OnAwake();

                animator = GetComponentInChildren<Animator>();
                _loadOperation = new List<AsyncOperation>();
                ActiveScenes = new List<string>();
            }

            private void Start()
            {
                DontDestroyOnLoad(gameObject);
                Hide();
            }
            public void ResetLevel()
            {
                BlockRaycast.enabled = true;

                StartCoroutine(ShowLoadingScreen(SceneManager.GetActiveScene().name));
            }

            public void LoadScene(string level)
            {
                BlockRaycast.enabled = true;
                StartCoroutine(ShowLoadingScreen(level));
            }

            public void LoadMainenu()
            {
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
                foreach (var item in ActiveScenes)
                {
                    UnloadLevel(item);
                }

                ActiveScenes.Clear();

                while (unloading)
                {
                    yield return new WaitForEndOfFrame();
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

                while (ao.isDone == false)
                {
                    UpdateProgress(ao.progress);
                    yield return null;
                }

                Hide();
            }

            private IEnumerator ShowLoadingScreen(string level)
            {
                ShowProgressBar();

                yield return new WaitForSeconds(2.0f);

                StartCoroutine(LoadSceneAsync(level));
            }

            private void ShowProgressBar()
            {
                if (animator)
                {
                    audioSouce.PlayOneShot(disapearSFX);
                    animator.ResetTrigger("Open");
                    animator.SetTrigger("Close");
                }
                Content.gameObject.SetActive(true);
                ProgressBarPanel.SetActive(true);
            }

            private void Hide()
            {
                if (animator)
                {
                    audioSouce.PlayOneShot(appearSFX);
                    animator.ResetTrigger("Close");
                    animator.SetTrigger("Open");
                }
                Content.gameObject.SetActive(false);
                BlockRaycast.enabled = false;
            }
        }
    }
}