using UnityEngine.SocialPlatforms;
using UnityEngine;
using EasyMobile;

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
public class GPServices : MonoSingleton<GPServices>
{
    protected override void Awake()
    {
        base.Awake();
      
        GameServices.UserLoginSucceeded -= OnUserLoginSucceeded;
        GameServices.UserLoginFailed -= OnUserLoginFailed;
    }
    private void Start()
    {
        GameServices.ManagedInit();
    }

    public void ManagedInit()
    {
    
    }

    public static bool IsInitialized()
    {
        //GameServices.IsInitialized()
        return false;
    }

    public static void ShowLeaderboard()
    {
        // Check for initialization before showing leaderboard UI
        if (GameServices.IsInitialized())
        {
            GameServices.ShowLeaderboardUI();
        }
        else
        {
#if UNITY_ANDROID
            GameServices.Init(); // start a new initialization process
#endif
        }
    }
    public static void ReportLeaderboards(long score, string leaderboard)
    {
        // Report a score of 100
        // EM_GameServicesConstants.Sample_Leaderboard is the generated name constant
        // of a leaderboard named "Sample Leaderboard"
        GameServices.ReportScore(score, EM_GameServicesConstants.Leaderboard_SurvivalMode);
    }
    public static void LoadLocalUserScore()
    {
        GameServices.LoadLocalUserScore(EM_GameServicesConstants.Leaderboard_SurvivalMode, OnLocalUserScoreLoaded);
    }

    public static void LoadScores()
    {
        GameServices.LoadScores(EM_GameServicesConstants.Leaderboard_SurvivalMode, 10, 20, TimeScope.Today, UserScope.Global, OnScoresLoaded);
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
        // Check for initialization before showing achievements UI
        if (GameServices.IsInitialized())
        {
            GameServices.ShowAchievementsUI();
        }
        else
        {
#if UNITY_ANDROID
            GameServices.Init(); // start a new initialization process
#endif
        }
    }

    public static void UnlockAchievement(string achievementId)
    {
        GameServices.UnlockAchievement(achievementId);
    }

    public static void ReportAchievementProgress(string achievementId, float value)
    {
        GameServices.ReportAchievementProgress(achievementId, value);
    }

    public static User GetUserInfo()
    {
        #if UNITY_ANDROID
        if (GameServices.LocalUser == null)
        {
            return null;
        }
        return new User(GameServices.LocalUser.image, GameServices.LocalUser.userName);
        #else
            return new User(null, "Over9000");
        #endif
    }
    public static void SignIn()
    {
        if (!GameServices.IsInitialized())
        {
            GameServices.Init();
        }
    }

    // Sign the user out on Android
    public static void SignOut()
    {
        if (GameServices.IsInitialized())
        {
            GameServices.SignOut();
        }
    }

    // Event handlers
    void OnUserLoginSucceeded()
    {
        Debug.Log("User logged in successfully.");
    }
    void OnUserLoginFailed()
    {

    }
}




