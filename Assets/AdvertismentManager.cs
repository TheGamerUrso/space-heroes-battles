using System.Collections;
using System.Collections.Generic;
using EasyMobile;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdvertismentManager : Singleton<AdvertismentManager>
{
    public static string gameId = "2725712";
    private string placementId = "banner";
    public static bool testMode = true;

    public void Initialize()
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

    protected override void OnAwake()
    {
        base.OnAwake();

        Initialize();
    }


    public void ShowBanner()
    {
        if (!Advertisement.isInitialized)
        {
            Advertisement.Initialize(gameId, testMode);

            StartCoroutine(ShowBannerWhenReady());
        }
    }

    public static void HideBanner()
    {
        Advertisement.Banner.Hide();
    }

    IEnumerator ShowBannerWhenReady()
    {
        while (!Advertisement.IsReady(placementId))
        {
            yield return new WaitForSeconds(0.5f);
        }
        Advertisement.Banner.SetPosition(BannerPosition.TOP_CENTER);
        Advertisement.Banner.Show(placementId);
    }



}
