using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardElement : MonoBehaviour
{
    [SerializeField] private RewardBox rewardBox;

    public void Claim()
    {
        rewardBox.Claim();
    }
}
