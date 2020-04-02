using EasyMobile;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TheGamerUrso.SceneLoader;
public class SplashScreen : MonoBehaviour
{
    public GooglePlayServicesManager googlePlayServicesManager;
    public float targetTime = 60.0f;
    public GameObject GoogleServicesNotify;

    private void Awake()
    {
        if (!RuntimeManager.IsInitialized())
            RuntimeManager.Init();
    }

    public void Start()
    {
        LoadGame();
    }

    private void Update()
    {
        //  targetTime -= Time.deltaTime;
        // targetTime = Mathf.Clamp(targetTime, 0, 60);
        // if (targetTime <= 0.0f)
        // {
        //     async.allowSceneActivation = true;
        //}
    }

    public void SignIn()
    {
        GooglePlayServicesManager.Instance.SignIn();
    }

    public void ContinueWithoutLogIn()
    {
        GoogleServicesNotify.SetActive(false);
    }

    public void LoadGame()
    {
        SceneLoader.Instance.LoadScene("Intro");
    }

    void OnUserLoginSucceeded()
    {
        Debug.Log("User logged in successfully.");
        GoogleServicesNotify.SetActive(false);
    }

    void OnUserLoginFailed()
    {
        Debug.Log("User login failed.");
        GoogleServicesNotify.SetActive(true);
    }

}