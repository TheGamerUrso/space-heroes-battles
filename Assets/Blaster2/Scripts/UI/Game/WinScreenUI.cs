using System;
using System.Collections;
using TheGamerUrso.Core;
using TMPro;
using UnityEngine;


public class WinScreenUI : UIView
{
    [SerializeField] private GameController gameController;
    [SerializeField] private TextMeshProUGUI ScoreText = null;

    public bool skip;
    private int score;
    private IAppService appService;

    private void Start()
    {
        appService = GameContext.Get<IAppService>();
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            skip = true;
        }
    }
    public void ShowGameResult(int Score)
    {
        score = Score;
        ScoreText.text = string.Format("{00:00000000}", 0); ;
        StartCoroutine(ShowGameResults());
    }

    private IEnumerator ShowGameResults()
    {
        float score = gameController.Score;
        float tempScore = 0;
        yield return new WaitForSeconds(2);

        while (tempScore < score)
        {
            tempScore = Mathf.Lerp(tempScore, score, .5f);
            ScoreText.text = string.Format("{00:00000000}", tempScore); ;
            if (skip)
            {
                tempScore = score;
            }
        }

        skip = false;
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