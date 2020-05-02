using System.Collections;
using System.Collections.Generic;
using EasyMobile;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdBanner : MonoBehaviour
{
    public IntroScreen introScreen;

    void Start()
    {
        Advertising.ShowBannerAd(BannerAdNetwork.UnityAds, AdPlacement.Default, BannerAdPosition.Top, BannerAdSize.SmartBanner);
    }


}
