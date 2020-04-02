using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TheGamerUrso.SceneLoader;
public class IntroScreen : MonoBehaviour
{
    public delegate void OnIntroClickContinue();
    public OnIntroClickContinue onIntroClickContinue;
    public GameObject[] Ships;
    private bool clicked;


    private void Awake()
    {
#if UNITY_EDITOR
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name.Equals("boot"))
            {
                Debug.Log("boot found skip");
                return;
            }
            Debug.Log("Boot not found Loading");
            SceneManager.LoadScene("boot", LoadSceneMode.Additive);

        }
#endif


    }

    void Start()
    {
        PlayerData playerData = DataController.GetPlayerData();

        for (int i = 0; i < Ships.Length; i++)
        {
            Ships[i].SetActive(false);
        }

        Ships[playerData.currentSelectedShip].SetActive(true);

        AudioManager.PlayMusic("Intro");
    }

    void Update()
    {
        if (!clicked && (Input.touchCount > 0 || Input.anyKey))
        {
            clicked = true;
            onIntroClickContinue?.Invoke();
            StartCoroutine(Fade());
        }
    }

    IEnumerator Fade()
    {

        yield return new WaitForSeconds(1.0f);
        if (SceneLoader.Instance)
            SceneLoader.Instance.LoadScene("Main");
    }
}
