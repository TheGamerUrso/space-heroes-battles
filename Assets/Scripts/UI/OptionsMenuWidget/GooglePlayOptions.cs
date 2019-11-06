using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GooglePlayOptions : BaseOptions
{
    public GameObject profile;
    public RawImage userIcon;
    public TextMeshProUGUI username;


    public Sprite[] buttonSprites;
    public Button signInBut;

    public Button AchievementBut;
    public Button LeaderboardBut;

    protected Coroutine signupCoroutine;
    public override void OnOptionEnter()
    {
        base.OnOptionEnter();
        if (Application.platform == RuntimePlatform.Android)
        {
            InitializeGooglePlayProfile();
        }
    }

    public void InitializeGooglePlayProfile()
    {
        User user = GooglePlayServicesManager.Instance.GetUserInfo();

        if (EasyMobile.GameServices.IsInitialized() || user != null)
        {
            profile.SetActive(true);

            username.text = user.username;

            signInBut.GetComponent<Image>().sprite = buttonSprites[1];
        }
        else if (!EasyMobile.GameServices.IsInitialized() || user == null)
        {
            profile.SetActive(false);
            username.text = string.Format("User{0}", Random.Range(1000, 9999));
            signInBut.GetComponent<Image>().sprite = buttonSprites[0];
        }
    }

    public void LogIn()
    {

        if (EasyMobile.GameServices.IsInitialized())
        {
            signInBut.GetComponent<Image>().sprite = buttonSprites[1];
            GooglePlayServicesManager.Instance.SignOut();
        }
        else
        {
            signInBut.GetComponent<Image>().sprite = buttonSprites[0];
            GooglePlayServicesManager.Instance.SignIn();
        }

        if (signupCoroutine != null)
        {
            StopCoroutine(signupCoroutine);
        }

        signupCoroutine = StartCoroutine(SignUp());
    }

    public IEnumerator SignUp()
    {
        while (!GooglePlayServicesManager.isInitialized)
        {
            if (GooglePlayServicesManager.isInitialized)
            {
                signInBut.GetComponent<Image>().sprite = buttonSprites[0];
                GooglePlayServicesManager.Instance.SignOut();
            }
            else
            {
                signInBut.GetComponent<Image>().sprite = buttonSprites[1];
                GooglePlayServicesManager.Instance.SignIn();
            }
            yield return new WaitForSeconds(1);
        }
        if (GooglePlayServicesManager.isInitialized)
        {
            signInBut.GetComponent<Image>().sprite = buttonSprites[0];
        }
        else
        {
            signInBut.GetComponent<Image>().sprite = buttonSprites[1];
        }

    }
    
    public void ShowLeaderboards()
    {
        GooglePlayServicesManager.Instance.ShowLeaderboards();
    }

    public void ShowAchievement()
    {
        GooglePlayServicesManager.Instance.ShowAchievementa();
    }
}
