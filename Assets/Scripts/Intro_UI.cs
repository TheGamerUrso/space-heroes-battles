using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intro_UI : MonoBehaviour
{
    public delegate void OnIntroClickContinue();
    public static OnIntroClickContinue OnIntroClickContinueHandled;
    private Animator animator;
    private bool clicked;
    private void Start()
    {
        animator = GetComponent<Animator>();
        AudioManager.SetMusic("Menu");
    }
    void Update()
    {
        if(!clicked && (Input.touchCount  > 0 || Input.anyKey))
        {
            clicked = true;
            StartCoroutine(Fade());
        }
    }

    IEnumerator Fade()
    {
        animator.SetTrigger("FadeOut");
        yield return new WaitForSeconds(1.0f);
        SceneLoader.Instance.LoadScene("Main");
    }

}

