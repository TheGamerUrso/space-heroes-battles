using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncomingBossMessageUI : BaseIncomingMessage
{
    [SerializeField] private Animator BossStageWarning;

    public override void RecieveTransmition(Action callback = null)
    {
        OnTransmisionEnded = callback;
        if (gameController.CurrentGameState != GameState.GAMEOVER)
            StartCoroutine(TranmisionEvent());
    }

    public override IEnumerator TranmisionEvent(bool playIntro = true)
    {
        AnimationClip[] animatorClipInfo = BossStageWarning.runtimeAnimatorController.animationClips;
        float length = animatorClipInfo[0].length;
        WaitForSeconds delay = new WaitForSeconds(length);

        BossStageWarning.gameObject.SetActive(true);

        yield return delay;

        BossStageWarning.gameObject.SetActive(false);
        OnTransmisionEnded?.Invoke();

    }
}
