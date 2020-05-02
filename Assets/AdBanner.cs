using System.Collections;
using System.Collections.Generic;
using EasyMobile;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdBanner : MonoBehaviour
{
    public IntroScreen introScreen;
    private string gameId = "2725712";

    public bool testMode = true;

    void Start()
    {
        AdvertismentManager.Instance.ShowBanner();
    }


}
