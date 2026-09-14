using System;
using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.Core;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
    private PlayerData playerData;

    [SerializeField] private TextMeshProUGUI Score = null;
    private string levelName;

    [Header("Win Widget Config")]
    public LevelObjectivesElement[] levelObjectives;

    public bool skip;
    private string scoreText;
    private IDataService dataService;
    private IAppService appService;

    private void Awake()
    {
        dataService = GameContext.Get<IDataService>();
        appService = GameContext.Get<IAppService>();
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            skip = true;
        }
    }
    public void ShowGameResult()
    {
        Scene scene = SceneManager.GetActiveScene();
        levelName = ((LevelEnum)scene.buildIndex).ToString();

        StartCoroutine(ShowGameResults());
    }

    private IEnumerator ShowGameResults()
    {
        var playerData = dataService.GetPlayerData();
        float score = GameController.Instance.Score;
        float tempScore = 0;
        yield return new WaitForSeconds(2);

        while (tempScore < score)
        {

            tempScore = Mathf.Lerp(tempScore, score, .5f);
            scoreText = string.Format("{00:00000000}", tempScore);
            Score.text = scoreText;
            if (skip)
            {
                tempScore = score;
            }
        }

        skip = false;

        yield return new WaitForSeconds(1);

        foreach (LevelObjectivesElement item in levelObjectives)
        {
            item.gameObject.SetActive(false);
        }
    }


    public void NextLevelButton()
    {
        appService.LoadMainMenu();
    }

    public void ReplayButton()
    {
        appService.ResetLevel();
    }

    public void LoadMainMenu()
    {
        appService.LoadMainMenu();
    }
}