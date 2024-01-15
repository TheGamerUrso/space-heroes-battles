using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LowHealthIndicator : MonoBehaviour
{
    [SerializeField] private PlayerShip playerShip;
    private bool Active;

    [SerializeField] private AnimationCurve animationCurve;
    [SerializeField] private Image image;
    private float alpha;

    [SerializeField] private Color defaultColor;
    [SerializeField] private Color HealedColor;

    [SerializeField] private AudioClip alarmSFX;
    [SerializeField] private AudioSource audioSource;

    private float count;
    private float timer;

    private void Start()
    {
        image.gameObject.SetActive(Active);

        defaultColor = image.color;
    }

    void Update()
    {

        if (playerShip.GetHealthPresentage() > .5f)
        {
            Active = false;
            count = 0;
            timer = 0;
        }
        else if (playerShip.GetHealthPresentage() <= .5f)
        {
            Active = true;
            image.color = defaultColor;
            count = 0;
            image.gameObject.SetActive(true);
        }

        if (Active)
        {
            Color c = image.color;
            c.a = Mathf.Lerp(c.a, animationCurve.Evaluate(Time.time), 1);
            image.color = c;
            if (!playerShip.IsAlive) return;
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            if (!playerShip.IsAlive) return;
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}
