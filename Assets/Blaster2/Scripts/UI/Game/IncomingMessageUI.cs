using System;
using System.Collections;
using UnityEngine;

public class IncomingMessageUI : BaseIncomingMessage
{
    public string[] transmitions;
    private bool PlayIntro = false;

    public override void RecieveTransmition(string[] transmitions,
        bool playIntro = true,Action callback = null)
    {
        this.OnTransmisionEnded = callback;
        this.transmitions = transmitions;
        this.PlayIntro = playIntro;
        if (transmitionCoroutine != null)
        {
            StopCoroutine(transmitionCoroutine);
        }
        if (gameController.CurrentGameState != GameState.GAMEOVER)
            transmitionCoroutine = StartCoroutine(TranmisionCoroutine());
    }

    public void PlayIncomingTransmision()
    {
        if (transmitionCoroutine != null)
        {
            StopCoroutine(transmitionCoroutine);
        }
        transmitionCoroutine = StartCoroutine(IncomingTransmitionCoroutine());
    }

    WaitForSeconds delay = new WaitForSeconds(.25f);
    WaitForSeconds SecondDelay = new WaitForSeconds(.5f);
    WaitForSeconds ThirdDelay = new WaitForSeconds(2.5f);

    public IEnumerator IncomingTransmitionCoroutine()
    {
        TransmitionText.text = "Transmition Incoming";
        audioSource.PlayOneShot(TransmitionSFX);
        panel.SetActive(true);
        for (int i = 0; i < 3; i++)
        {
            panel.SetActive(false);
            yield return delay;
            panel.SetActive(true);
            yield return delay;
        }

        panel.SetActive(false);

    }


    public override IEnumerator TranmisionCoroutine()
    {
        if (PlayIntro)
        {
            yield return IncomingTransmitionCoroutine();
            panel.SetActive(true);
        }

        foreach (var transmition in transmitions)
        {
            panel.SetActive(true);
            TransmitionText.text = transmition;
            yield return ThirdDelay;
            panel.SetActive(false);
            yield return delay;
        }

        yield return SecondDelay;
        panel.SetActive(false);
        OnTransmisionEnded?.Invoke();
    }
}