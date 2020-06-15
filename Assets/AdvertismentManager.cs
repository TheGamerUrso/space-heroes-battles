using System.Collections;
using System.Collections.Generic;
using EasyMobile;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdvertismentManager: Singleton<AdvertismentManager>
{

    public static string gameId = "2725712";
    private static string bannerId = "banner";
    private static string rewardVideoId = "rewardedVideo";
    public static bool testMode = false;

    public static void Initialize()
    {
        if (!Advertisement.isInitialized)
        {
            Advertisement.Initialize(gameId, testMode);
        }

        // Grants the module-level consent for the Advertising module.
        if (Advertising.DataPrivacyConsent == ConsentStatus.Unknown)
        {
            Advertising.GrantDataPrivacyConsent();
        }
    }


    public static void ShowBanner()
    {
        if (!Advertisement.isInitialized)
        {
            Advertisement.Initialize(gameId, testMode);
        }
    }

    public static void HideBanner()
    {
        Advertisement.Banner.Hide();
    }

    public static bool AdvertismentReady()
    {
        return Advertisement.IsReady(rewardVideoId);
    }

    public static void ShowRewardedVideo()
    {
        // Check if rewarded ad is ready
        bool isReady = Advertising.IsRewardedAdReady();

        // Show it if it's ready
        if (isReady)
        {
            Advertising.ShowRewardedAd();
        }
    }
    public static void ShowAdvertisment()
    {
        // Check if interstitial ad is ready
        bool isReady = Advertising.IsInterstitialAdReady();

        // Show it if it's ready
        if (isReady)
        {
            Advertising.ShowInterstitialAd();
        }
    }

}
