using System;
using System.Collections;
using UnityEngine;

public class IncomingMessageUI : BaseIncomingMessage
{
    public string[] transmitions;
    private bool PlayIntro = false;

    public override void RecieveTransmition(Action callback = null)
    {
        OnTransmisionEnded = callback;
        TransmitionText.text = "Transmition Incoming";

        if (transmitionCoroutine != null)
        {
            StopCoroutine(transmitionCoroutine);
        }

        if (gameController.CurrentGameState != GameState.GAMEOVER)
            transmitionCoroutine = StartCoroutine(TranmisionEvent(PlayIntro));
    }

    public void RecieveTransmition(string[] transmitions,
        bool playIntro = true,Action callback = null)
    {
        this.transmitions = transmitions;
        this.PlayIntro = playIntro;
        RecieveTransmition(callback);  
    }



    public override IEnumerator TranmisionEvent(bool playIntro = true)
    {
        WaitForSeconds delay = new WaitForSeconds(.25f);
        WaitForSeconds SecondDelay = new WaitForSeconds(.5f);
        WaitForSeconds ThirdDelay = new WaitForSeconds(2.5f);

        if (playIntro)
        {            
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
            yield return SecondDelay;

            panel.SetActive(true);
        }
        for (int i = 0; i < transmitions.Length; i++)
        {
            panel.SetActive(true);
            TransmitionText.text = transmitions[i];
            yield return ThirdDelay;
            panel.SetActive(false);
            yield return delay;
        }

        yield return SecondDelay;
        panel.SetActive(false);
        OnTransmisionEnded?.Invoke();
    }
}