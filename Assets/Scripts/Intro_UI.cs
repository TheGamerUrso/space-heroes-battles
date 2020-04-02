using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intro_UI : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        GameObject.FindObjectOfType<IntroScreen>().onIntroClickContinue += () =>
        {
            animator.SetTrigger("FadeOut");
        };
    }
}

