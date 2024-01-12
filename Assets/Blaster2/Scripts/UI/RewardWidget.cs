using CodeMonkey.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public enum RewardTypeEnum
{
    Gold = 0, XP = 1, HEALTH = 2, SHIELD = 3, POWERUP = 4, SUPER = 5
}

public class RewardWidget : MonoBehaviour
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
    private void OnDestroy()
    {
        Events.ClaimReward = null;
        Events.ClaimedReward = null;
    }

    private void Start()
    {
        Events.ClaimReward = ClaimReward;
        Events.ClaimedReward = RewardClaimed;
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

    public bool
    RewardClaimed()
    {
        return false;
    }

    public void Show()
    {
        GameController.Instance.UseSlowMo = false;
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

        PlayerData playerData = PersistantData.GetPlayerData();
        PlayerShipData playerShipData = playerData.GetCurrentPlayerShipData();
        PlayerShip playerShip = PlayerManager.GetPlayer();
        string textToShow = "No Reward";

        switch (rewardType)
        {
            case RewardTypeEnum.Gold:
                int rewardCoin = Random.Range(50, 300);
                textToShow = rewardText[(int)RewardTypeEnum.Gold].Replace("%", rewardCoin.ToString());
                playerData.AddCoin(rewardCoin);
                RewardText.text = textToShow;
                break;
            case RewardTypeEnum.XP:
                float xpReward = Random.Range(50, 200);
                textToShow = rewardText[(int)RewardTypeEnum.XP].Replace("%", xpReward.ToString());
                xpReward = Mathf.Clamp(xpReward, 1, playerShipData.xpToLevel);
                playerData.EarnXP(xpReward);
                break;
            case RewardTypeEnum.HEALTH:
                textToShow = rewardText[(int)RewardTypeEnum.HEALTH];
                playerShip.Heal(playerShip.MaxHealth / 2);
                break;
            case RewardTypeEnum.SHIELD:
                textToShow = rewardText[(int)RewardTypeEnum.SHIELD];
                playerShip.InstallShield();
                break;
            case RewardTypeEnum.POWERUP:
                textToShow = rewardText[(int)RewardTypeEnum.POWERUP];
                playerShip.PowerUpCollected();
                break;
            case RewardTypeEnum.SUPER:
                textToShow = rewardText[(int)RewardTypeEnum.SUPER];
                float power = playerData.PowerUpLevel + .5f;
                playerData.SetSuperMeter(power);
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
        GameController.Instance.UseSlowMo = true;
    }

}
