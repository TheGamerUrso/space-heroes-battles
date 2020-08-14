using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class RewardBox : MonoBehaviour
{
    private bool claimed;
    private bool opened;
    private bool notAvailable = false;
    [HideInInspector] private int ID;
    [HideInInspector] public Camera CameraReview;
    [SerializeField] private RenderTexture renderTexture;
    [SerializeField] private Animator Box;
    private RewardTypeEnum rewardTypeEnum;

    private float timer = 2;

    public void SetReward(RewardTypeEnum rewardTypeEnum)
    {
        claimed = false;
        this.rewardTypeEnum = rewardTypeEnum;
    }
    private void OnEnable()
    {
        Box.SetTrigger("Reset");
        Box.ResetTrigger("Open");
    }
    void Start()
    {
        CameraReview.targetTexture = renderTexture;
        Events.ClaimReward += ClaimReward;
    }
    private void Update()
    {
        if (claimed && !notAvailable)
        {
            timer -= Time.deltaTime;
            if (timer < 0 && !opened)
            {
                opened = true;
                Events.ClaimReward?.Invoke(ID, rewardTypeEnum);
            }
        }
    }


    public void Claim()
    {
        if (!claimed)
        {
            claimed = true;
            Box.SetTrigger("Open");
            Events.ClaimReward?.Invoke(ID, rewardTypeEnum);
        }
    }

    public void ClaimReward(int Id, RewardTypeEnum rewardTypeEnum)
    {
        if (ID.Equals(Id))
        {
            Box.SetTrigger("Close");
            claimed = true;
        }
        notAvailable = true;
    }
}
