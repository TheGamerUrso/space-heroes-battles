using EasyMobile;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashScreen : MonoBehaviour
{
    public GooglePlayServicesManager googlePlayServicesManager;
    private AsyncOperation async;
    public float targetTime = 60.0f;

    [SerializeField] private Image Loading;
    private bool AllowGameStart = false;


    public GameObject GoogleServicesNotify;
    private void Awake()
    {
        if (!RuntimeManager.IsInitialized())
            RuntimeManager.Init();
    }

    public void Start()
    {
        Application.runInBackground = true;

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
#if UNITY_ANDROID
        if (GameServices.IsInitialized())
        {
            async.allowSceneActivation = true;
        }
#elif UNITY_EDITOR
        async.allowSceneActivation = true;
#endif

    }

    public void SignIn()
    {
        GooglePlayServicesManager.Instance.SignIn();

    }

    public void ContinueWithoutLogIn()
    {
        GoogleServicesNotify.SetActive(false);
        async.allowSceneActivation = true;
    }

    public void LoadGame()
    {
        async = SceneManager.LoadSceneAsync("Main");
        async.allowSceneActivation = false;
    }

    void OnUserLoginSucceeded()
    {
        Debug.Log("User logged in successfully.");
        GoogleServicesNotify.SetActive(false);
        async.allowSceneActivation = true;
    }

    void OnUserLoginFailed()
    {
        Debug.Log("User login failed.");
        GoogleServicesNotify.SetActive(true);
    }

}