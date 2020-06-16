using System.Collections;
using UnityEngine;

public class TransmitionWidget : MonoBehaviour
{
    public bool IncomingTransmition;

    public GameObject TransmitionWidgetPrefab;
    public TMPro.TextMeshProUGUI TransmitionText;
    public Animator BossStageWarning;
    public string[] transmitions;

   public AudioClip TransmitionSFX;
    public AudioSource audioSource;



    public void RecieveTransmition(string[] transmitions,bool boss = false)
    {
        TransmitionText.text = "Transmition Incoming";

        this.transmitions = transmitions;
        if (IncomingTransmition == false)
        {
            if (boss)
            {
                if(!GameSession.IsGameOver)
                    StartCoroutine(WarningBossIncomingEvent());
            }
            else {
                audioSource.PlayOneShot(TransmitionSFX);
                if(!GameSession.IsGameOver)
                    StartCoroutine(TranmisionEvent());
            }
        }
    }


    private IEnumerator WarningBossIncomingEvent()
    {
      

        AnimationClip[] animatorClipInfo = BossStageWarning.runtimeAnimatorController.animationClips;
        float length = animatorClipInfo[0].length;
        WaitForSeconds delay = new WaitForSeconds(length);

        IncomingTransmition = true;

        BossStageWarning.gameObject.SetActive(true);

        yield return delay;

        BossStageWarning.gameObject.SetActive(false);
        IncomingTransmition = false;
    }

    private IEnumerator TranmisionEvent()
    {
        WaitForSeconds delay = new WaitForSeconds(.25f);
        WaitForSeconds SecondDelay = new WaitForSeconds(.5f);
        WaitForSeconds ThirdDelay = new WaitForSeconds(2.5f);

        IncomingTransmition = true;
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
        IncomingTransmition = false;
    }
}