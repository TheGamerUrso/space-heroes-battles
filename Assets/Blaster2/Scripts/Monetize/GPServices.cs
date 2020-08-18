using UnityEngine.SocialPlatforms;
using UnityEngine;

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
public class GPServices : Singleton<GPServices>
{
    public GPServices()
    {
        //GameServices.UserLoginSucceeded -= OnUserLoginSucceeded;
        //GameServices.UserLoginFailed -= OnUserLoginFailed;
    }

    public void ManagedInit()
    {
        //GameServices.ManagedInit();
    }

    public static bool IsInitialized()
    {
        //GameServices.IsInitialized()
        return false;
    }

    public static void ShowLeaderboard()
    {
        //        // Check for initialization before showing leaderboard UI
        //        if (GameServices.IsInitialized())
        //        {
        //            GameServices.ShowLeaderboardUI();
        //        }
        //        else
        //        {
        //#if UNITY_ANDROID
        //            GameServices.Init(); // start a new initialization process
        //#elif UNITY_IOS
        //Debug.Log("Cannot show leaderboard UI: The user is not logged in to Game Center.");
        //#endif
        //        }
    }
    public static void ReportLeaderboards(long score, string leaderboard)
    {
        // Report a score of 100
        // EM_GameServicesConstants.Sample_Leaderboard is the generated name constant
        // of a leaderboard named "Sample Leaderboard"
        // GameServices.ReportScore(score, EM_GameServicesConstants.Leaderboard_Survival_Mode);
    }
    public static void LoadLocalUserScore()
    {
        // GameServices.LoadLocalUserScore(EM_GameServicesConstants.Leaderboard_Survival_Mode, OnLocalUserScoreLoaded);
    }

    public static void LoadScores()
    {
        // GameServices.LoadScores(EM_GameServicesConstants.Leaderboard_Survival_Mode, 10, 20, TimeScope.Today, UserScope.Global, OnScoresLoaded);
    }

    public static void OnScoresLoaded(string leaderboardName, IScore[] scores)
    {
        if (scores != null && scores.Length > 0)
        {
            Debug.Log("Loaded " + scores.Length + " from leadeboard " + leaderboardName);
            foreach (IScore score in scores)
            {
                Debug.Log("Score: " + score.value + "; rank: " + score.rank);
            }
        }
        else
        {
            Debug.Log("No score loaded.");
        }
    }

    private static void OnLocalUserScoreLoaded(string leaderboardName, IScore score)
    {
        if (score != null)
        {
            Debug.Log("Your score is: " + score.value);
        }
        else
        {
            Debug.Log("You don't have any score reported to leaderboard " + leaderboardName);
        }
    }

    //Achievements
    public static void ShowAchievements()
    {
        //        // Check for initialization before showing achievements UI
        //        if (GameServices.IsInitialized())
        //        {
        //            GameServices.ShowAchievementsUI();
        //        }
        //        else
        //        {
        //#if UNITY_ANDROID
        //            GameServices.Init(); // start a new initialization process
        //#elif UNITY_IOS
        //Debug.Log("Cannot show achievements UI: The user is not logged in to Game Center.");
        //#endif
        //        }
    }

    public static void UnlockAchievement(string achievementId)
    {
        //GameServices.UnlockAchievement(achievementId);
    }

    public static void ReportAchievementProgress(string achievementId, float value)
    {
        //GameServices.ReportAchievementProgress(achievementId, value);
    }

    public static User GetUserInfo()
    {
        //#if UNITY_ANDROID
        //        if (GameServices.LocalUser == null)
        //        {
        //            return null;
        //        }
        //        return new User(GameServices.LocalUser.image, GameServices.LocalUser.userName);
        return new User(null, "Over9000");

    }
    public static void SignIn()
    {
        //if (!GameServices.IsInitialized())
        //{
        //    GameServices.Init();
        //}
    }

    // Sign the user out on Android
    public static void SignOut()
    {
        //if (GameServices.IsInitialized())
        //{
        //    GameServices.SignOut();
        //}
    }
}




