using EasyMobile;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
public class User
{

    public Texture2D userIcon;
    public string username;


    public User(Texture2D userIcon, string username)
    {
        this.userIcon = userIcon;
        this.username = username;
    }


}
public class GooglePlayServicesManager : MonoBehaviour
{
    public static GooglePlayServicesManager Instance;

    public static bool isInitialized;

    private void Awake()
    {
        if (Instance == false)
        {
            Instance = this;

            if (!RuntimeManager.IsInitialized())
                RuntimeManager.Init();

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void Start()
    {
        // Managed init respects the Max Login Requests value
        if (!GameServices.IsInitialized())
        {
            GameServices.Init();
        }
    }

    public bool GetInitialized()
    {
        return GameServices.IsInitialized();
    }

    public User GetUserInfo()
    {
        return new User(GameServices.LocalUser.image, GameServices.LocalUser.userName);
    }



    public void ReportLeaderboards(long score, string leaderboard)
    {
        if (GameServices.IsInitialized())
        {

#if UNITY_ANDROID
            GameServices.ReportScore(score, leaderboard);
#elif UNITY_IOS
    Debug.Log("Cannot show achievements UI: The user is not logged in to Game Center.");
#endif
        }
    }

    public void ReportAchivementProgress(string achievement, float ammount)
    {
        if (GameServices.IsInitialized())
        {
#if UNITY_ANDROID
            if (string.IsNullOrEmpty(achievement))
            {
                return;
            }

            GameServices.ReportAchievementProgress(achievement, ammount);
#elif UNITY_IOS
    Debug.Log("Cannot show achievements UI: The user is not logged in to Game Center.");
#endif
        }
    }

    public void UnlockAchivement(string achievement)
    {
        if (GameServices.IsInitialized())
        {
#if UNITY_ANDROID
            if (string.IsNullOrEmpty(achievement))
            {
                return;
            }

            GameServices.UnlockAchievement(achievement);
#elif UNITY_IOS
    Debug.Log("Cannot show achievements UI: The user is not logged in to Game Center.");
#endif
        }
    }

    public void UnlockAchievement(int achievement)
    {
        if (GameServices.IsInitialized())
        {
#if UNITY_ANDROID
            // Unlock an achievement
            // EM_GameServicesConstants.Sample_Achievement is the generated name constant
            // of an achievement named "Sample Achievement"
            string achievementToUnlock = "";
            switch (achievement)
            {
                case 1:
                    achievementToUnlock = EasyMobile.EM_GameServicesConstants.Achievement_Prologue_Completed;
                    break;
                case 2:
                    achievementToUnlock = EasyMobile.EM_GameServicesConstants.Achievement_Prologue_Completed;
                    break;

                case 3:
                    achievementToUnlock = EasyMobile.EM_GameServicesConstants.Achievement_Prologue_Completed;
                    break;

                case 4:
                    achievementToUnlock = EasyMobile.EM_GameServicesConstants.Achievement_Prologue_Completed;
                    break;

                case 5:
                    achievementToUnlock = EasyMobile.EM_GameServicesConstants.Achievement_Prologue_Completed;
                    break;

                case 6:
                    achievementToUnlock = EasyMobile.EM_GameServicesConstants.Achievement_Prologue_Completed;
                    break;
                case 7:
                    achievementToUnlock = EasyMobile.EM_GameServicesConstants.Achievement_Prologue_Completed;
                    break;
                case 8:

                    achievementToUnlock = EasyMobile.EM_GameServicesConstants.Achievement_Prologue_Completed;
                    break;
                case 9:
                    achievementToUnlock = EasyMobile.EM_GameServicesConstants.Achievement_Prologue_Completed;
                    break;
                default:
                    break;
            }

            if (string.IsNullOrEmpty(achievementToUnlock))
            {
                return;
            }

            GameServices.UnlockAchievement(achievementToUnlock);
#elif UNITY_IOS
        Debug.Log("Cannot show achievements UI: The user is not logged in to Game Center.");
#endif
        }
    }

    public void ShowAchievementa()
    {
#if UNITY_ANDROID
        // Check for initialization before showing achievements UI
        if (GameServices.IsInitialized())
        {
            GameServices.ShowAchievementsUI();
        }
        else
        {
            GameServices.Init();    // start a new initialization process
#elif UNITY_IOS
    Debug.Log("Cannot show achievements UI: The user is not logged in to Game Center.");
#endif
    }

    public void AddScore(long score)
    {
#if UNITY_ANDROID
        GameServices.ReportScore(score, EM_GameServicesConstants.Leaderboard_Survival_Mode);
#elif UNITY_IOS
    Debug.Log("Cannot show achievements UI: The user is not logged in to Game Center.");
#endif
    }

    public void ShowLeaderboards()
    {
        if (GameServices.IsInitialized())
        {
#if UNITY_ANDROID
            // Check for initialization before showing leaderboard UI
            if (GameServices.IsInitialized())
            {
                GameServices.ShowLeaderboardUI();
            }
            else
            {

                GameServices.Init();    // start a new initialization process
#elif UNITY_IOS
    Debug.Log("Cannot show leaderboard UI: The user is not logged in to Game Center.");
#endif
        }
    }

    public void SignIn()
    {
#if UNITY_ANDROID
        GameServices.ManagedInit();
#elif UNITY_IOS
    Debug.Log("Cannot show leaderboard UI: The user is not logged in to Game Center.");
#endif
    }

    public void SignOut()
    {
#if UNITY_ANDROID
        GameServices.SignOut();
#elif UNITY_IOS
        Debug.Log("Cannot show leaderboard UI: The user is not logged in to Game Center.");
#endif
    }
}



