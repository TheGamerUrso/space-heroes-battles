using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LowHealthIndicator : MonoBehaviour
{
    public PlayerShip playerShip;
    public bool Active;

    public AnimationCurve animationCurve;
    public Image image;
    public float alpha;

    public Color defaultColor;
    public Color HealedColor;

    public AudioClip alarmSFX;
    public AudioSource audioSource;

    private float count;
    private float timer;

    private void Start()
    {
        image.gameObject.SetActive(Active);

        defaultColor = image.color;
    }

    void Update()
    {
        if (Active)
        {
            Color c = image.color;
            c.a = Mathf.Lerp(c.a, animationCurve.Evaluate(Time.time), 1);
            image.color = c;

            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                timer = 1;
                count++;
                audioSource.PlayOneShot(alarmSFX);                
            }
        }

        if (playerShip.GetHealthPresentage() <= 0 || playerShip.GetHealthPresentage() > .5f)
        {
            if (Active)
            {
                Active = false;
                count = 0;
                timer = 0;
            }
        }
        else if (playerShip.GetHealthPresentage() <= .5f)
        {
            if (!Active)
            {
                Active = true;
                image.color = defaultColor;
                count = 0;
                image.gameObject.SetActive(true);
            }
        }
    }
}
