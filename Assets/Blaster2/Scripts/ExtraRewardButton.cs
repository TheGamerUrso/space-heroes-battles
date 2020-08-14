using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using EasyMobile;

[RequireComponent(typeof(Button))]
public class ExtraRewardButton : MonoBehaviour
{
    Button myButton;
    private void OnDestroy()
    {
        Advertising.RewardedAdCompleted -= RewardedAdCompletedHandler;

        Advertising.RewardedAdSkipped -= RewardedAdSkippedHandler;
    }
    private void OnEnable()
    {
        Advertising.RewardedAdCompleted += RewardedAdCompletedHandler;

        Advertising.RewardedAdSkipped += RewardedAdSkippedHandler;
    }
    // Event handler called when a rewarded ad has completed
    void RewardedAdCompletedHandler(RewardedAdNetwork network, AdPlacement location)
    {
        // Reward the user for watching the ad to completion.
        PlayerData playerData = PersistantData.GetPlayerData();
        int coin = Game.CoinPicked;
        coin *= 2;
        playerData.AddCoin(coin);
        Popup.Show(Popup.popupType.message, "Rewardeed \n" + coin);

        gameObject.SetActive(false);
    }

    // Event handler called when a rewarded ad has been skipped
    void RewardedAdSkippedHandler(RewardedAdNetwork network, AdPlacement location)
    {
        // Do not reward the user for skipping the ad.
        Popup.Show(Popup.popupType.message, "No Reward");
    }

    void Start()
    {
        myButton = GetComponent<Button>();

        // Set interactivity to be dependent on the Placement’s status:
        myButton.interactable = AdvertismentManager.AdvertismentReady();
    }

    public void ShowAdvertisement()
    {
        AdvertismentManager.ShowRewardedVideo();
    }

}
