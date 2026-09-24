using System;
using System.Collections;
using TheGamerUrso.Core;
using UnityEngine;

public class TransmitionWidget : MonoBehaviour
{
    private event Action OnTransmisionEnded;
    public GameObject TransmitionWidgetPrefab;
    public TMPro.TextMeshProUGUI TransmitionText;
    public Animator BossStageWarning;
    public string[] transmitions;

    public AudioClip TransmitionSFX;
    public AudioSource audioSource;
    private Coroutine transmitionCoroutine;

    public void RecieveTransmition(string[] transmitions,bool PlayIntro = true,Action callback = null)
    {
        OnTransmisionEnded = callback;
        TransmitionText.text = "Transmition Incoming";
        this.transmitions = transmitions;

        if (transmitionCoroutine != null)
        {
            StopCoroutine(transmitionCoroutine);
        }

        if (!GameController.Instance.IsGameOver)
            transmitionCoroutine = StartCoroutine(TranmisionEvent(PlayIntro));
    }

    public void BossWarning(Action callback = null)
    {
        OnTransmisionEnded = callback;
        if (!GameController.Instance.IsGameOver)
            StartCoroutine(WarningBossIncomingEvent());
    }

    private IEnumerator WarningBossIncomingEvent()
    {
        AnimationClip[] animatorClipInfo = BossStageWarning.runtimeAnimatorController.animationClips;
        float length = animatorClipInfo[0].length;
        WaitForSeconds delay = new WaitForSeconds(length);

        BossStageWarning.gameObject.SetActive(true);

        yield return delay;

        BossStageWarning.gameObject.SetActive(false);
        OnTransmisionEnded?.Invoke();
    }

    private IEnumerator TranmisionEvent(bool playIntro = true)
    {
        WaitForSeconds delay = new WaitForSeconds(.25f);
        WaitForSeconds SecondDelay = new WaitForSeconds(.5f);
        WaitForSeconds ThirdDelay = new WaitForSeconds(2.5f);

        if (playIntro)
        {            
            audioSource.PlayOneShot(TransmitionSFX);
            TransmitionWidgetPrefab.SetActive(true);
            for (int i = 0; i < 3; i++)
            {
                TransmitionWidgetPrefab.SetActive(false);
                yield return delay;
                TransmitionWidgetPrefab.SetActive(true);
                yield return delay;
            }

            TransmitionWidgetPrefab.SetActive(false);
            yield return SecondDelay;

            TransmitionWidgetPrefab.SetActive(true);
        }
        for (int i = 0; i < transmitions.Length; i++)
        {
            TransmitionWidgetPrefab.SetActive(true);
            TransmitionText.text = transmitions[i];
            yield return ThirdDelay;
            TransmitionWidgetPrefab.SetActive(false);
            yield return delay;
        }

        yield return SecondDelay;
        TransmitionWidgetPrefab.SetActive(false);
        OnTransmisionEnded?.Invoke();
    }
}