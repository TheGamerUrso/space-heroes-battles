using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoSingleton<SceneLoader>
{
    private class LoadingMonoBehaviour : MonoBehaviour { }

    public static Action OnLoadCallbak;
    private static AsyncOperation LoadingAsyncOperation;
    public Image BlockRaycast;
    public GameObject ProgressBarPanel;
    public Image progressBar;

    public GameObject Content;
    public Animator animator;

    public override void Init()
    {
        base.Init();

        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        Hide();
    }

    public static float GetLoadingProgress()
    {
        if(LoadingAsyncOperation != null)
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
        GameManager.instance.PauseTheGame(false);
        StartCoroutine(ShowLoadingScreen("Main"));
    }

    public IEnumerator ShowLoadingScreen(string level)
    {
        ShowProgressBar();

        yield return new WaitForSeconds(2.0f);

        StartCoroutine(LoadSceneAsync(level));
    }

    private IEnumerator LoadSceneAsync(string level)
    {
        LoadingAsyncOperation = SceneManager.LoadSceneAsync(level);

        while (LoadingAsyncOperation.isDone == false)
        {
            progressBar.fillAmount = GetProgress();
            yield return null;
        }

        if (level.Equals("Main"))
        {
            GameManager.instance.SetState(GameStates.Menu);
        }

        Hide();
    }
    public static float GetProgress()
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
    public void ShowProgressBar()
    {
        if (animator)
        {
            animator.ResetTrigger("Open");
            animator.SetTrigger("Close");
        }
        Content.gameObject.SetActive(true);
        ProgressBarPanel.SetActive(true);
    }

    public void Hide()
    {
        if (animator)
        {
            animator.ResetTrigger("Close");
            animator.SetTrigger("Open");
        }
        Content.gameObject.SetActive(false);
        BlockRaycast.enabled = false;
    }
}