using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideOpen : MonoBehaviour
{
    string openSlide = "Shoot";

    Animator slideAnimator;
    // Start is called before the first frame update
    void Start()
    {
        slideAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey) TriggerSlide();
    }

    public void TriggerSlide()
    {
    slideAnimator.SetTrigger(openSlide);
    }
}
