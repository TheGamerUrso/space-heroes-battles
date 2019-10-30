using System.Collections;
using UnityEngine;

public class TransmitionWidget : MonoBehaviour
{
    public bool IncomingTransmition;

    public GameObject TransmitionWidgetPrefab;
    public TMPro.TextMeshProUGUI TransmitionText;

    public string[] transmitions;

    public void RecieveTransmition(string[] transmitions)
    {
        TransmitionText.text = "Transmition Incoming";
        this.transmitions = transmitions;
        if (IncomingTransmition == false)
        {
            StartCoroutine(TranmisionEvent());
        }
    }

    private IEnumerator TranmisionEvent()
    {
        IncomingTransmition = true;
        TransmitionWidgetPrefab.SetActive(true);
        for (int i = 0; i < 3; i++)
        {
            TransmitionWidgetPrefab.SetActive(false);
            yield return new WaitForSeconds(.25f);
            TransmitionWidgetPrefab.SetActive(true);
            yield return new WaitForSeconds(.25f);
        }

        TransmitionWidgetPrefab.SetActive(false);
        yield return new WaitForSeconds(.5f);

        TransmitionWidgetPrefab.SetActive(true);
        for (int i = 0; i < transmitions.Length; i++)
        {
            TransmitionWidgetPrefab.SetActive(true);
            TransmitionText.text = transmitions[i];
            yield return new WaitForSeconds(2.5f);
            TransmitionWidgetPrefab.SetActive(false);
            yield return new WaitForSeconds(.25f);
        }

        yield return new WaitForSeconds(.5f);
        TransmitionWidgetPrefab.SetActive(false);
        IncomingTransmition = false;
    }
}