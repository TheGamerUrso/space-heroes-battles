using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFade : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private AnimationCurve animationCurve;
    private bool Fade = false;
    private float alpha;
    private float fadeTimer = 0;
    [SerializeField] private float speed = 1;

    private void OnEnable()
    {
        Fade = true;
    }

    void Update()
    {
        if (Fade)
        {
            fadeTimer += Time.deltaTime * speed;
            alpha = Mathf.Lerp(alpha, animationCurve.Evaluate(fadeTimer), 1);
            canvasGroup.alpha = alpha;
            if (alpha >= .9f)
            {
                alpha = 1;
                Fade = false;
            }
        }
    }
}
