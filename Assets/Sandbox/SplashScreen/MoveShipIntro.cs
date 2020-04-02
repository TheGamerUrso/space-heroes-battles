using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveShipIntro : MonoBehaviour
{
    public Animator animator;

    private void Start()
    {
        GameObject.FindObjectOfType<IntroScreen>().onIntroClickContinue += () => { animator.SetTrigger("Go"); };
    }
     
}
