using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intro_UI : MonoBehaviour
{
    public delegate void OnIntroClickContinue();
    public static OnIntroClickContinue OnIntroClickContinueHandled;
    public GameObject MainUI;
    private Animator animator;
    
    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if(Input.touchCount  > 0 || Input.anyKey)
        {
            StartCoroutine(Fade());
        }
    }

    IEnumerator Fade()
    {
        animator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(1.0f);
        MainUI.SetActive(true);
        OnIntroClickContinueHandled?.Invoke();
    }

}

