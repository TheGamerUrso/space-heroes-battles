using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActivateRandomLevel : MonoBehaviour
{

    public GameObject WrapTunnelFX;
    public Image Fade;
    public TextMeshProUGUI text;

    public float cooldown;

    public GameObject[] levels;

    public bool fadeIn;
    public bool fadeOut;

    private Color c;

    SurvivalMode survivalMode;
    private bool active;
    private bool firstTime = true;
    public float speed = 0.5f;
    public int previousLevelLoaded;

    private void Start()
    {
        survivalMode = GameObject.FindObjectOfType<SurvivalMode>();
        survivalMode.OnWaveEnded = ActivateHyperdrive;
    }

    public bool ActivateHyperdrive()
    {
        if (firstTime && !active)
        {
            firstTime = false;
            StartCoroutine(Hyperdrive());
        }

        if (!firstTime && active)
        {
            return true;
        }

        firstTime = true;
        return false;
    }

    private void Update()
    {
        if (fadeIn)
        {
            c = Fade.color;
            c.a += speed * Time.deltaTime;
            Fade.color = c;
        }
    }


    IEnumerator Hyperdrive()
    {
        active = true;
        text.text = "Hyperdrive in";

        yield return new WaitForSeconds(.5f);

        text.text = "Hyperdrive in\n" + "3";

        yield return new WaitForSeconds(.5f);

        text.text = "Hyperdrive in\n" + "2";


        yield return new WaitForSeconds(.5f);

        text.text = "Hyperdrive in\n" + "1";

        yield return new WaitForSeconds(.5f);
        text.text = "";

        fadeIn = true;
        while (c.a < 1)
        {
            yield return null;
        }

        fadeIn = false;

        c = Fade.color;
        c.a = 0;
        Fade.color = c;

        WrapTunnelFX.SetActive(true);

        yield return new WaitForSeconds(1.0f);

        for (int i = 0; i < levels.Length; i++)
        {
            levels[i].SetActive(false);
        }

        int randLevel = 0;
        randLevel = Random.Range(0, levels.Length);
        levels[randLevel].SetActive(true);

        yield return new WaitForSeconds(1.0f);

        WrapTunnelFX.SetActive(false);
        active = false;
        //Debug.Log("Done");
    }

}
