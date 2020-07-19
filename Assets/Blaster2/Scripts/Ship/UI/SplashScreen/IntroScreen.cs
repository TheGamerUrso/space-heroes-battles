using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroScreen : MonoBehaviour
{
    public delegate void OnIntroClickContinue();
    public OnIntroClickContinue onIntroClickContinue;
    public GameObject[] Ships;
    private bool clicked;
    public GameObject pressToContinue;
    public AudioSource sfx;


    void Start()
    {
        PlayerData playerData = PersistantData.GetPlayerData();

        for (int i = 0; i < Ships.Length; i++)
        {
            Ships[i].SetActive(false);
        }

        Ships[playerData.CurrrentSelectedShip].SetActive(true);

        AudioManager.PlayMusic("Intro");
    }

    void Update()
    {
        if (!clicked && (Input.touchCount > 0 || Input.anyKey))
        {
            clicked = true;
            onIntroClickContinue?.Invoke();
            StartCoroutine(Fade());
            pressToContinue.gameObject.SetActive(false);
            sfx.Play();
        }
    }

    IEnumerator Fade()
    {

        yield return new WaitForSeconds(1.0f);
        if (GameManager.Instance)
            GameManager.Instance.LoadMainenu();
    }
}
