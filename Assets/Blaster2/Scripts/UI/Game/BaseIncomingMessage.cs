using System;
using System.Collections;
using UnityEngine;

public abstract class BaseIncomingMessage : UIView
{
    [SerializeField] protected GameController gameController;
    public Action OnTransmisionEnded;
    [SerializeField] protected TMPro.TextMeshProUGUI TransmitionText;
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected AudioClip TransmitionSFX;
    protected Coroutine transmitionCoroutine;
    public abstract void RecieveTransmition(Action callback = null);

    public abstract IEnumerator TranmisionEvent(bool playIntro = true);

}
