using UnityEngine;

public class RewardItemEvent
{
    public RewardTypeEnum rewardType;
    public object reward;
    public RewardItemEvent(RewardTypeEnum rewardTypeEnum, object reward)
    {
        rewardType = rewardTypeEnum;
        this.reward = reward;
    }
}
