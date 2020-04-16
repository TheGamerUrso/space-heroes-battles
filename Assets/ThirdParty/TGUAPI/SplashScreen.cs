using EasyMobile;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TheGamerUrso.SceneLoader;
public class SplashScreen : MonoBehaviour
{

    public Image progressbar;

    private void Awake()
    {
        if (!RuntimeManager.IsInitialized())
            RuntimeManager.Init();
    }

    public void Start()
    {
        LoadGame();
    }

    public void LoadGame()
    {
        StartCoroutine(LoadNext());
    }

    IEnumerator LoadNext()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync("boot");

        while (!async.isDone)
        {
            progressbar.fillAmount = async.progress;
            yield return null;
        }
    }
}