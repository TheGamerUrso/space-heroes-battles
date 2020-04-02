using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBar : MonoBehaviour
{
    public GameObject[] Bars;
    public bool playing;
    public float speed;

    private void Start()
    {
        speed = UnityEngine.Random.Range(0.01f, 0.03f);
        for (int i = 0; i < Bars.Length; i++)
        {
            Bars[i].SetActive(false);
        }
    }
    private void Update()
    {


        if (!playing)
        {
            playing = true;
            StartCoroutine(Load());
        }
    }
    IEnumerator Load()
    {

        List<GameObject> bars = new List<GameObject>();

        int rand = UnityEngine.Random.Range(0, Bars.Length);
        for (int i = 0; i < rand; i++)
        {
            Bars[i].SetActive(true);
            bars.Add(Bars[i]);
            yield return new WaitForSeconds(speed);
        }


        for (int i = bars.Count -1; i > 0; i--)
        {
            bars[i].SetActive(false);

            yield return new WaitForSeconds(speed);
        }

        yield return new WaitForSeconds(speed);
        playing = false;
    }
}
