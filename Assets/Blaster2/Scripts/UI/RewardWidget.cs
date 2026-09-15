using CodeMonkey.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.Core;
using TMPro;
using UnityEditor.MPE;
using UnityEngine;
using Random = UnityEngine.Random;

public enum RewardTypeEnum
{
    Gold = 0, XP = 1, HEALTH = 2, SHIELD = 3, POWERUP = 4, SUPER = 5
}

public class RewardWidget : UIView
{
    private int RewardBoxSelected;

    public GameObject widgetPanel;

    public GameObject rewardPanel;

    public RewardBox[] rewardBoxes;

    public RewardElement[] rewardElement;

    public RewardTypeEnum[] rewards;

    private RewardTypeEnum rewardType;

    public string[] rewardText = { "% gold earned", "% xp earned", "ship repaired", "Shield Installed", "power up", "Decrease super cooldown" };
    public GameObject rewardResultPanel;
    public TextMeshProUGUI RewardText;
    private bool IsWaitingInput;
    protected IDataService dataService;
    protected IEventService eventService;

   [SerializeField] protected GameController gameController;
    private void Start()
    {
        dataService = GameContext.Get<IDataService>();
        eventService = GameContext.Get<IEventService>();

        rewards = new RewardTypeEnum[3];
        rewardPanel.SetActive(false);
        rewardResultPanel.SetActive(false);
    }

    public void ClaimReward(int Id, RewardTypeEnum rewardTypeEnum)
    {
        RewardBoxSelected = Id;
        rewardType = rewardTypeEnum;
        StartCoroutine(ClaimRewarded());
    }

    public void GetNewRewards()
    {
        for (int i = 0; i < rewards.Length; i++)
        {
            rewards[i] = (RewardTypeEnum)Random.Range(0, Enum.GetValues(typeof(RewardTypeEnum)).Length);
            rewardBoxes[i].SetReward(rewards[i]);
            rewardBoxes[i].CloseChest();
        }
    }

    public bool RewardClaimed()
    {
        return false;
    }

    public override void Show()
    {
        base.Show();
        gameController.IsSlowMo = false;
        rewardPanel.SetActive(true);
        StartCoroutine(ShowWaveReward());
    }

    IEnumerator ShowWaveReward()
    {
        GetNewRewards();
        Time.timeScale = 0.0f;
        while (IsWaitingInput)
        {
            yield return new WaitForSeconds(1.0f);
        }
        Time.timeScale = 1.0f;
    }

    IEnumerator ClaimRewarded()
    {
        yield return new WaitForSeconds(1.0f);
        string textToShow = "No Reward";

        switch (rewardType)
        {
            case RewardTypeEnum.Gold:
                int rewardCoin = Random.Range(50, 300);
                eventService.Publish(new RewardItemEvent(rewardType, rewardCoin));
                textToShow = rewardText[(int)RewardTypeEnum.Gold].Replace("%", rewardCoin.ToString());
                break;
            case RewardTypeEnum.XP:
                float xpReward = Random.Range(50, 200);
                eventService.Publish(new RewardItemEvent(rewardType, xpReward));
                textToShow = rewardText[(int)RewardTypeEnum.XP].Replace("%", xpReward.ToString());
                break;
            case RewardTypeEnum.HEALTH:
                textToShow = rewardText[(int)RewardTypeEnum.HEALTH];
                eventService.Publish(new RewardItemEvent(rewardType, 0));
                break;
            case RewardTypeEnum.SHIELD:
                textToShow = rewardText[(int)RewardTypeEnum.SHIELD];
                eventService.Publish(new RewardItemEvent(rewardType, 0));
                break;
            case RewardTypeEnum.POWERUP:
                textToShow = rewardText[(int)RewardTypeEnum.POWERUP];
                eventService.Publish(new RewardItemEvent(rewardType, 0));
                break;
            case RewardTypeEnum.SUPER:
                textToShow = rewardText[(int)RewardTypeEnum.SUPER];
                eventService.Publish(new RewardItemEvent(rewardType,0));
                break;
        }
 
        RewardText.text = textToShow;
        rewardResultPanel.SetActive(true);
        rewardPanel.SetActive(false);


        yield return new WaitForSeconds(3.0f);


        rewardResultPanel.SetActive(false);


        yield return new WaitForSeconds(1.0f);
        rewardBoxes[RewardBoxSelected].CloseChest();
        yield return new WaitForSeconds(1.0f);
        rewardResultPanel.SetActive(false);
        RewardClaimed();

        IsWaitingInput = false;
        GameController.Instance.IsSlowMo = true;
    }

}
