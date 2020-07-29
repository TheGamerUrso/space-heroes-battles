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
        }

        if (playerShip.GetHealthPresentage() <= 0 || playerShip.GetHealthPresentage() > .5f)
        {
            if (Active)
            {
                StopAllCoroutines();
                Active = false;
            }
        }
        else if (playerShip.GetHealthPresentage() <= .5f)
        {
            if (!Active)
            {
                StopAllCoroutines();
                Active = true;
                StartCoroutine(PlayAlarm());
                image.gameObject.SetActive(true);
            }
        }
    }

    IEnumerator PlayAlarm()
    {
        image.color = defaultColor;

        var count = 0;

        while (count <= 3)
        {
            audioSource.PlayOneShot(alarmSFX);
            yield return new WaitForSeconds(1.0f);
            count++;
        }
    }


}
