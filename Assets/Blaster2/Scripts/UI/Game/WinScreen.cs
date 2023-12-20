using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
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
        float score = Game.Score;
        float tempScore = 0;
        yield return new WaitForSeconds(2);

        while (tempScore < score)
        {

            tempScore = Mathf.Lerp(tempScore, score, .5f);
            scoreText = string.Format("{00:0000000000}", tempScore);
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
        GameManager.Instance.LoadMainenu();
    }

    public void ReplayButton()
    {
        GameManager.Instance.ResetLevel();
    }

    public void LoadMainMenu()
    {
        GuiManager.Instance.LoadMainMenu();
    }
}