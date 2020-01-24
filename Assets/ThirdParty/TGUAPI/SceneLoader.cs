using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : Singleton<SceneLoader>
{
    private static AsyncOperation LoadingAsyncOperation;
    private Animator animator;
    public Image progressBar;

    public Image BlockRaycast;
    public GameObject ProgressBarPanel;
    public GameObject Content;

    public string currentLevelLoaded;

    public override void Init()
    {
        base.Init();
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        Hide();
    }
    
    public void AllowSceneActivation()
    {
        LoadingAsyncOperation.allowSceneActivation = true;
    }

    private float GetLoadingProgress()
    {
        if (LoadingAsyncOperation != null)
        {
            return LoadingAsyncOperation.progress;
        }
        else
        {
            return 1f;
        }
    }

    public void LoadScene(string level)
    {
        BlockRaycast.enabled = true;
        StartCoroutine(ShowLoadingScreen(level));
    }

    public void LoadMainenu()
    {
        StartCoroutine(ShowLoadingScreen("Main"));
    }

    private IEnumerator ShowLoadingScreen(string level)
    {
        ShowProgressBar();

        yield return new WaitForSeconds(2.0f);

        StartCoroutine(LoadSceneWithDelay(level));
    }
    private IEnumerator LoadSceneWithDelay(string level)
    { 
        LoadingAsyncOperation = SceneManager.LoadSceneAsync(level);

        while (LoadingAsyncOperation.isDone == false)
        {
            progressBar.fillAmount = GetProgress();
            System.GC.Collect();
            yield return null;
        }

 
        Hide();
    }

    private IEnumerator LoadSceneAsync(string level)
    {
        LoadingAsyncOperation = SceneManager.LoadSceneAsync(level);
        LoadingAsyncOperation.completed += OnSceneLoadCompleted;
        while (LoadingAsyncOperation.isDone == false)
        {
            progressBar.fillAmount = GetProgress();
            yield return null;
        }

    }

    private static float GetProgress()
    {
        if (LoadingAsyncOperation != null)
        {
            return LoadingAsyncOperation.progress;
        }
        else
        {
            return 1f;
        }
    }

    private void ShowProgressBar()
    {
        if (animator)
        {
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
            animator.ResetTrigger("Close");
            animator.SetTrigger("Open");
        }
        Content.gameObject.SetActive(false);
        BlockRaycast.enabled = false;
    }

    void OnSceneLoadCompleted(AsyncOperation ao)
    {
        if (currentLevelLoaded.Contains("Level"))
        {
            AudioManager.PlayRandomMusic(true);
            Application.targetFrameRate = 60;
        }
    }
}