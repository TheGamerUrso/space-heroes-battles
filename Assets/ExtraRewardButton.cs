using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using EasyMobile;

[RequireComponent(typeof(Button))]
public class ExtraRewardButton : MonoBehaviour
{

    private string gameId = "2725712";

    Button myButton;
    public string myPlacementId = "rewardedVideo";

    // Subscribe to rewarded ad events
    void OnEnable()
    {
        Advertising.RewardedAdCompleted += RewardedAdCompletedHandler;
        Advertising.RewardedAdSkipped += RewardedAdSkippedHandler;
    }
    // Unsubscribe events
    void OnDisable()
    {
        Advertising.RewardedAdCompleted -= RewardedAdCompletedHandler;
        Advertising.RewardedAdSkipped -= RewardedAdSkippedHandler;
    }

    // Event handler called when a rewarded ad has completed
    void RewardedAdCompletedHandler(RewardedAdNetwork network, AdLocation location)
    {
        // Reward the user for watching the ad to completion.
        PlayerData playerData = PersistantData.GetPlayerData();
        int coin = GameSession.coinEarnInGame;
        coin *= 2;
        playerData.AddCoin(coin);
        Popup.Show(Popup.popupType.message, "Rewardeed \n" + coin);

        gameObject.SetActive(false);
    }

    // Event handler called when a rewarded ad has been skipped
    void RewardedAdSkippedHandler(RewardedAdNetwork network, AdLocation location)
    {
        // Do not reward the user for skipping the ad.
        Popup.Show(Popup.popupType.message, "No Reward");
    }

    void Start()
    {
        myButton = GetComponent<Button>();

        // Set interactivity to be dependent on the Placement’s status:
        myButton.interactable = Advertisement.IsReady(myPlacementId);
    }

    // Implement a function for showing a rewarded video ad:
    public void ShowRewardedVideo()
    {
        // Check if rewarded ad is ready
        bool isReady = Advertising.IsRewardedAdReady();

        // Show it if it's ready
        if (isReady)
        {
            Advertising.ShowRewardedAd();
        }
    }

}
