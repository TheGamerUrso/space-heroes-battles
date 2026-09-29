using System;
using System.Collections;
using TheGamerUrso.Core;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using static IntermediateRewardManager;
using Random = UnityEngine.Random;

public enum RewardTypeEnum
{
    Gold = 0, XP = 1, HEALTH = 2, SHIELD = 3, POWERUP = 4, SUPER = 5
}

public class RewardUI : UIView
{
    private int RewardBoxSelected;

    public GameObject rewardClaimedPanel;
    public GameObject rewardPanel;

    public RewardBox[] rewardBoxes;

    public RewardElement[] rewardElement;
    public TextMeshProUGUI RewardText;


    public IntermediateRewardManager intermediateRewardManager;

    private void Start()
    {
        panel.SetActive(false);
        rewardPanel.SetActive(true);
        rewardClaimedPanel.SetActive(false);
        intermediateRewardManager.OnNewRewardGenerated += IntermediateRewardManager_OnNewRewardGenerated;
    }

    private void IntermediateRewardManager_OnNewRewardGenerated(RewardTypeEnum[] rewards)
    {
        for (int i = 0; i < rewards.Length; i++)
        {
            rewardBoxes[i].SetReward(rewards[i]);
            rewardBoxes[i].CloseChest();
        }
    }

    public void ClaimRewardButton(int Id)
    {
        RewardBoxSelected = Id; // Cache index for closing the correct chest
        StartCoroutine(ClaimRewarded());
        intermediateRewardManager.ClaimReward(Id);
    }

    public bool RewardClaimed()
    {
        return false;
    }

    [ContextMenu("Debug_Show")]
    public override void Show()
    {
        base.Show();
        intermediateRewardManager.SetState(IntermediateRewardState.INITIALIZE);
        rewardPanel.SetActive(true);
    }

    IEnumerator ClaimRewarded()
    {
        yield return new WaitForSeconds(1.0f);
        RewardText.text = intermediateRewardManager.TextToShow;
        rewardClaimedPanel.SetActive(true);
        rewardPanel.SetActive(false);


        yield return new WaitForSeconds(3.0f);


        rewardClaimedPanel.SetActive(false);


        yield return new WaitForSeconds(1.0f);
        rewardBoxes[RewardBoxSelected].CloseChest();
        yield return new WaitForSeconds(1.0f);
        rewardClaimedPanel.SetActive(false);
    }

}
