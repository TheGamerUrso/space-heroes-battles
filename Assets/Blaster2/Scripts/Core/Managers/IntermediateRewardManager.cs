using System;
using System.Text;
using TheGamerUrso.Core;
using UnityEngine;

public class IntermediateRewardManager : MonoBehaviour
{
    public enum IntermediateRewardState
    {
        NONE,INITIALIZE,WAITING,CLAIMED
    }

    public event Action<IntermediateRewardState> OnIntermediateRewardStateChanged;
    public event Action<RewardTypeEnum[]> OnNewRewardGenerated;
    private IntermediateRewardState intermediateRewardState;

    public int RewardBoxSelected;
    public RewardTypeEnum[] rewards;

    private RewardTypeEnum rewardType;
    public string[] rewardTextList = { "% gold earned", "% xp earned", "ship repaired", "Shield Installed", "power up", "Decrease super cooldown" };
    private bool IsWaitingInput;
    protected IDataService dataService;
    protected IEventService eventService;
    [SerializeField] protected GameController gameController;
    private float timer = 1;
    public string TextToShow { get; private set; }

    void Start()
    {
        dataService = GameContext.Get<IDataService>();
        eventService = GameContext.Get<IEventService>();
        rewards = new RewardTypeEnum[3];
    }

    public void SetState(IntermediateRewardState intermediateRewardState)
    {
        this.intermediateRewardState = intermediateRewardState;
        OnIntermediateRewardStateChanged?.Invoke(intermediateRewardState);
    }

    public void Update()
    {
        switch (intermediateRewardState)
        {
            case IntermediateRewardState.NONE:
                break;
            case IntermediateRewardState.INITIALIZE:
                GenerateNewRewards();
                gameController.IsSlowMo = false;
                SetState(IntermediateRewardState.WAITING);
                break;
            case IntermediateRewardState.WAITING:
                break;
            case IntermediateRewardState.CLAIMED:
                timer -= Time.deltaTime;
                if (timer <= 1)
                {
                    SetState(IntermediateRewardState.NONE);
                    OnIntermediateRewardStateChanged?.Invoke(intermediateRewardState);
                }
                break;
        }
    }
    public void GenerateNewRewards()
    {
        for (int i = 0; i < rewards.Length; i++)
        {
            rewards[i] = (RewardTypeEnum)UnityEngine.Random.Range(0, Enum.GetValues(typeof(RewardTypeEnum)).Length);
        }

    }

    public void ClaimReward(int rewardIndex)
    {
        timer = 6;
        RewardBoxSelected = rewardIndex;
        rewardType = rewards[rewardIndex];

        SetState(IntermediateRewardState.CLAIMED);
        TextToShow = "No Reward";
        switch (rewardType)
        {
            case RewardTypeEnum.Gold:
                int rewardCoin = UnityEngine.Random.Range(50, 300);
                eventService.Publish(new RewardItemEvent(rewardType, rewardCoin));
                TextToShow = rewardTextList[(int)RewardTypeEnum.Gold].Replace("%", rewardCoin.ToString());
                break;
            case RewardTypeEnum.XP:
                float xpReward = UnityEngine.Random.Range(50, 200);
                eventService.Publish(new RewardItemEvent(rewardType, xpReward));
                TextToShow = rewardTextList[(int)RewardTypeEnum.XP].Replace("%", xpReward.ToString());
                break;
            case RewardTypeEnum.HEALTH:
                TextToShow = rewardTextList[(int)RewardTypeEnum.HEALTH];
                eventService.Publish(new RewardItemEvent(rewardType, 0));
                break;
            case RewardTypeEnum.SHIELD:
                TextToShow = rewardTextList[(int)RewardTypeEnum.SHIELD];
                eventService.Publish(new RewardItemEvent(rewardType, 0));
                break;
            case RewardTypeEnum.POWERUP:
                TextToShow = rewardTextList[(int)RewardTypeEnum.POWERUP];
                eventService.Publish(new RewardItemEvent(rewardType, 0));
                break;
            case RewardTypeEnum.SUPER:
                TextToShow = rewardTextList[(int)RewardTypeEnum.SUPER];
                eventService.Publish(new RewardItemEvent(rewardType, 0));
                break;
        }
        gameController.IsSlowMo = true;
    }
}
