using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncomingBossMessageUI : BaseIncomingMessage
{
    [SerializeField] private Animator BossStageWarning;

    public override void RecieveTransmition(string[] transmitions,
        bool playIntro = true, Action callback = null)
    {
        OnTransmisionEnded = callback;
        if (gameController.CurrentGameState != GameState.GAMEOVER)
            StartCoroutine(TranmisionCoroutine());
    }

    public override IEnumerator TranmisionCoroutine()
    {
        AnimationClip[] animatorClipInfo = BossStageWarning.runtimeAnimatorController.animationClips;
        float length = animatorClipInfo[0].length;
        WaitForSeconds delay = new WaitForSeconds(length);

        Show();

        yield return delay;

        Hide();
        OnTransmisionEnded?.Invoke();

    }
}
