using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewRocketeer : MonoBehaviour
{
    float hatchetOpenTime = 1f, hatchetCloseTime = 2f, rocketTime;
    bool hatchetOpen = false;
    string openSwitch = "rocketOpen";

    Animator rocketeerAnimator;
    // Start is called before the first frame update
    void Start()
    {
        rocketeerAnimator = GetComponent<Animator>();
        rocketTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hatchetOpen)
        {
            if (rocketTime + hatchetOpenTime < Time.time)
            {
                rocketeerAnimator.SetBool(openSwitch, true);
                hatchetOpen = true;
                rocketTime += hatchetOpenTime;
            }
        }
        else
        {
            if (rocketTime + hatchetCloseTime < Time.time)
            {
                rocketeerAnimator.SetBool(openSwitch, false);
                hatchetOpen = false;
                rocketTime += hatchetCloseTime;
            }
        }
    }
}
