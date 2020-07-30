using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ActivateRandomLevel : MonoBehaviour
{
    [SerializeField] private Camera cameraMain;
    [SerializeField] private GameObject WrapTunnelFX;
    [SerializeField] private Image Fade;
    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] private float cooldown;

    [SerializeField] private GameObject[] levels;
    private List<GameObject> ListOfLevels = new List<GameObject>();
    private bool fadeIn;
    private bool fadeOut;

    private Color c;

    SurvivalMode survivalMode;
    private bool active;
    private bool firstTime = true;
    [SerializeField] private float speed = 0.5f;
    [SerializeField] private int previousLevelLoaded;

    [SerializeField] private LightMapSwitcher lightMapSwitcher;
    [SerializeField] private LayerMask defaultLayer;
    [SerializeField] private LayerMask hyperspaceLayer;

    private GameObject previousLevel;

    private void Awake()
    {
        cameraMain = Camera.main;
        survivalMode = GameObject.FindObjectOfType<SurvivalMode>();
        Events.OnWaveEnded = ActivateHyperdrive;
        defaultLayer = cameraMain.cullingMask;


        foreach (GameObject item in levels)
        {
            GameObject levelItem = Instantiate(item, transform, false);
            levelItem.name = item.name;
            ListOfLevels.Add(levelItem);
        }
    }

    private void Start()
    {
        ChooseNewLevel();
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
        cameraMain.cullingMask = hyperspaceLayer;
        fadeIn = false;

        c = Fade.color;
        c.a = 0;
        Fade.color = c;

        WrapTunnelFX.SetActive(true);

        yield return new WaitForSeconds(1.0f);

        Asteroids[] asteroids = GameObject.FindObjectsOfType<Asteroids>();
        for (int i = 0; i < asteroids.Length; i++)
        {
            asteroids[i].gameObject.SetActive(false);
        }

        ChooseNewLevel();

        yield return new WaitForSeconds(1.0f);
        cameraMain.cullingMask = defaultLayer;
        WrapTunnelFX.SetActive(false);
        active = false;
    }

    public void ChooseNewLevel()
    {
        for (int i = 0; i < ListOfLevels.Count; i++)
        {
            ListOfLevels[i].SetActive(false);
        }

        int randLevel = 0;
        randLevel = Random.Range(0, levels.Length);
        GameObject levelToLoad = ListOfLevels[randLevel];

        if (previousLevel != null && previousLevel == levelToLoad)
        {
            if (randLevel == 0)
            {
                randLevel += 1;
            }
            else if (randLevel < levels.Length)
            {
                randLevel -= 1;
            }
            else
            {
                var randomNum = Random.Range(1, 100);
                if (randomNum >= 50)
                {
                    randLevel += 1;
                }
                else
                {
                    randLevel -= 1;
                }
            }
            levelToLoad = ListOfLevels[randLevel];
        }

        levelToLoad.SetActive(true);

        previousLevel = levelToLoad;

        lightMapSwitcher.SetLevelLightmap(ListOfLevels[randLevel].name);
    }

}
