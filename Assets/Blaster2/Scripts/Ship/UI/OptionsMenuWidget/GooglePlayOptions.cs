using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GooglePlayOptions : BaseOptions
{
    public GameObject profile;
    public TextMeshProUGUI username;


    public Sprite[] buttonSprites;
    public Button signInBut;

    public Button AchievementBut;
    public Button LeaderboardBut;

    protected Coroutine signupCoroutine;
    public override void Start()
    {
        base.Start();
        if (Application.platform == RuntimePlatform.Android)
        {
            InitializeGooglePlayProfile();
        }
    }

    public void InitializeGooglePlayProfile()
    {
        User user = GPServices.GetUserInfo();

        if (GPServices.IsInitialized() || user != null)
        {
            profile.SetActive(true);

            username.text = user.username;

            signInBut.GetComponent<Image>().sprite = buttonSprites[1];
        }
        else if (!GPServices.IsInitialized() || user == null)
        {
            profile.SetActive(false);
            username.text = string.Format("User{0}", Random.Range(1000, 9999));
            signInBut.GetComponent<Image>().sprite = buttonSprites[0];
        }
    }

    public void LogIn()
    {

        if (GPServices.IsInitialized())
        {
            signInBut.GetComponent<Image>().sprite = buttonSprites[1];
            GPServices.SignOut();
        }
        else
        {
            signInBut.GetComponent<Image>().sprite = buttonSprites[0];
            GPServices.SignIn();
        }

        if (signupCoroutine != null)
        {
            StopCoroutine(signupCoroutine);
        }

        signupCoroutine = StartCoroutine(SignUp());
    }

    public IEnumerator SignUp()
    {
        while (!GPServices.IsInitialized())
        {
            if (GPServices.IsInitialized())
            {
                signInBut.GetComponent<Image>().sprite = buttonSprites[0];
                GPServices.SignOut();
            }
            else
            {
                signInBut.GetComponent<Image>().sprite = buttonSprites[1];
                GPServices.SignIn();
            }
            yield return new WaitForSeconds(1);
        }

        if (GPServices.IsInitialized())
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
        GPServices.ShowLeaderboard();
    }

    public void ShowAchievement()
    {
        GPServices.ShowAchievements();
    }
}
