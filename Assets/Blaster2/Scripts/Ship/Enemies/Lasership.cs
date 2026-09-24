using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lasership : Enemy
{
    float hatchetOpenTime = 1f, hatchetCloseTime = 2f, rocketTime;
    bool hatchetOpen = false;
    string openSwitch = "rocketOpen";

    public Animator rocketeerAnimator;

    public override void Start()
    {
        base.Start();
        rocketTime = Time.time;
        hatchetOpen = true;
    }

    public override void Update()
    {
        base.Update();
        if(!hatchetOpen)
        {
            if (rocketTime+hatchetOpenTime < Time.time)
            {
                rocketeerAnimator.SetBool(openSwitch, true);
                //hatchetOpen = true;
                rocketTime += hatchetOpenTime;
            }                
        }
        else
        {
            if (rocketTime + hatchetCloseTime < Time.time )
            {
                rocketeerAnimator.SetBool(openSwitch, false);
                //hatchetOpen = false;
                rocketTime += hatchetCloseTime;
            }
        }
    }
}
