using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnIntroClickContinue();
public class Intro : MonoBehaviour
{
    public static OnIntroClickContinue OnIntroClickContinueHandled;
    private bool clicked;

    private void Start()
    {
        AudioManager.PlayMusic("Intro");
    }

    void Update()
    {
        if (!clicked && (Input.touchCount > 0 || Input.anyKey))
        {
            clicked = true;
            StartCoroutine(Fade());
        }
    }

    IEnumerator Fade()
    {
        OnIntroClickContinueHandled?.Invoke();
        yield return new WaitForSeconds(1.0f);
        SceneLoader.Instance.LoadScene("Main");
    }
}
