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
public class GooglePlayServicesManager:Singleton<GooglePlayServicesManager>
{
    public static void Initialize()
    {
        if (!RuntimeManager.IsInitialized())
            RuntimeManager.Init();

        //Managed init respects the Max Login Requests value
        if (!GameServices.IsInitialized())
        {
            GameServices.Init();
        }
    }

    public static bool GetInitialized()
    {
        return GameServices.IsInitialized();
    }

    public static User GetUserInfo()
    {
#if UNITY_ANDROID
        return new User(GameServices.LocalUser.image, GameServices.LocalUser.userName);
#else
        return new User(null, "Over9000");
#endif
    }



    public static void ReportLeaderboards(long score, string leaderboard)
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


    public static void ReportAchivementProgress(string achievement, float ammount)
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


    public static void UnlockAchivement(string achievement)
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

    public static void UnlockAchievement(int achievement)
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
                    achievementToUnlock = EasyMobile.EM_GameServicesConstants.Achievement_Level_1_Completed;
                    break;

                case 3:
                    achievementToUnlock = EasyMobile.EM_GameServicesConstants.Achievement_Level_2_Completed;
                    break;

                case 4:
                    achievementToUnlock = EasyMobile.EM_GameServicesConstants.Achievement_Level_3_Completed;
                    break;

                case 5:
                    achievementToUnlock = EasyMobile.EM_GameServicesConstants.Achievement_Level_4_Completed;
                    break;

                case 6:
                    achievementToUnlock = EasyMobile.EM_GameServicesConstants.Achievement_Level_5_Completed;
                    break;
                case 7:
                    achievementToUnlock = EasyMobile.EM_GameServicesConstants.Achievement_Level_6_Completed;
                    break;
                case 8:

                    achievementToUnlock = EasyMobile.EM_GameServicesConstants.Achievement_Level_7_Completed;
                    break;
                case 9:
                    achievementToUnlock = EasyMobile.EM_GameServicesConstants.Achievement_Level_8_Completed;
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

    public static void ShowAchievementa()
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
        }
#elif UNITY_IOS
            Debug.Log("Cannot show achievements UI: The user is not logged in to Game Center.");
#endif
    }

    public static void AddScore(long score)
    {
#if UNITY_ANDROID
        GameServices.ReportScore(score, EM_GameServicesConstants.Leaderboard_Survival_Mode);
#elif UNITY_IOS
            Debug.Log("Cannot show achievements UI: The user is not logged in to Game Center.");
#endif
    }

    public static void ShowLeaderboards()
    {

#if UNITY_ANDROID
        if (GameServices.IsInitialized())
        {

            // Check for initialization before showing leaderboard UI
            if (GameServices.IsInitialized())
            {
                GameServices.ShowLeaderboardUI();
            }
            else
            {

                GameServices.Init();    // start a new initialization process
            }
        }
#elif UNITY_IOS
        else{
            Debug.Log("Cannot show leaderboard UI: The user is not logged in to Game Center.");


        }
#endif
    }



    public static void SignIn()
    {
#if UNITY_ANDROID
        GameServices.ManagedInit();
#elif UNITY_IOS
            Debug.Log("Cannot show leaderboard UI: The user is not logged in to Game Center.");
#endif
    }

    public static void SignOut()
    {
#if UNITY_ANDROID
        GameServices.SignOut();
#elif UNITY_IOS
                Debug.Log("Cannot show leaderboard UI: The user is not logged in to Game Center.");
#endif
    }
}




