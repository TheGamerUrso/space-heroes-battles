using System.Collections;
using System.Collections.Generic;
using EasyMobile;
using UnityEngine;

public class AdvertismentManager : MonoBehaviour
{

    void Start()
    {
        // Grants the module-level consent for the Advertising module.
        Advertising.GrantDataPrivacyConsent(AdNetwork.UnityAds);

        // Revokes the module-level consent of the Advertising module.
        Advertising.RevokeDataPrivacyConsent(AdNetwork.UnityAds);

        // Reads the current module-level consent of the Advertising module.
        ConsentStatus moduleConsent = Advertising.DataPrivacyConsent;
    }



}
