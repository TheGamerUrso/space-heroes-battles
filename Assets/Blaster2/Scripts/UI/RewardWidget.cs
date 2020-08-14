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


    public string[] rewardText = { "X Gold Earned", "X XP Earned", "SHIP REPAIRED", "SHIELD INSTALLED", "POWER UP", "DECREASE SUPER COOLDOWN" };
    public GameObject reward;
    public TextMeshProUGUI RewardText;
    private void OnDestroy()
    {
        Events.ClaimReward -= ClaimReward;
        Events.ClaimedReward -= RewardClaimed;
    }

    private void OnEnable()
    {
        Events.ClaimReward = ClaimReward;
        Events.ClaimedReward = RewardClaimed;

        rewardPanel.SetActive(true);
        reward.SetActive(false);
        GetNewRewards();

        Time.timeScale = 1.0f;
        Game.UseSlowMo = false;
    }

    private void Start()
    {
        rewards = new RewardTypeEnum[3];
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
        }
    }

    public bool RewardClaimed()
    {
        return false;
    }

    IEnumerator ClaimRewarded()
    {
        yield return new WaitForSeconds(2.0f);

        rewardBoxes[RewardBoxSelected].CloseChest();
        
        PlayerData playerData = PersistantData.GetPlayerData();
        PlayerShipData playerShipData = playerData.GetCurrentPlayerShipData();
        PlayerShip playerShip = PlayerManager.GetPlayer();
        string textToShow = "No Reward";

        switch (rewardType)
        {
            case RewardTypeEnum.Gold:
                int rewardCoin = Random.Range(50, 300);
                textToShow = rewardText[(int)RewardTypeEnum.Gold].Replace("X", rewardCoin.ToString());
                playerData.AddCoin(rewardCoin);
                RewardText.text = textToShow;
                break;
            case RewardTypeEnum.XP:
                float xpReward = Random.Range(50, 200);
                textToShow = rewardText[(int)RewardTypeEnum.XP].Replace("X", xpReward.ToString());
                xpReward = Mathf.Clamp(xpReward, 1, playerShipData.xpToLevel);
                playerData.EarnXP(xpReward);
                break;
            case RewardTypeEnum.HEALTH:
                textToShow = rewardText[(int)RewardTypeEnum.HEALTH];
                playerShip.Heal(playerShip.MaxHealth / 2);
                break;
            case RewardTypeEnum.SHIELD:
                textToShow = rewardText[(int)RewardTypeEnum.SHIELD];
                playerShip.InstallShieldModule();
                break;
            case RewardTypeEnum.POWERUP:
                textToShow = rewardText[(int)RewardTypeEnum.POWERUP];
                playerShip.PowerUpCollected();
                break;
            case RewardTypeEnum.SUPER:
                textToShow = rewardText[(int)RewardTypeEnum.SUPER];
                playerData.SetSuperMeter(.5f);
                break;
        }

        RewardText.text = textToShow;
        reward.SetActive(true);

        yield return new WaitForSeconds(2.0f);

        reward.SetActive(false);
        widgetPanel.SetActive(false);

        yield return new WaitForSeconds(3.0f);

        RewardClaimed();
        Game.UseSlowMo = true;
    }

}
