using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WinScreen : MonoBehaviour
{
    private PlayerData playerData;

    [SerializeField] private TextMeshProUGUI Score = null;
    private string levelName;

    [Header("Win Widget Config")]
    public LevelObjectivesElement[] levelObjectives;
    private LevelObjectiveData[] levelObjectiveDatas;


    public GameObject VideoRewardAd;
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
        levelName = "Level" + GameManager.LevelIndexSelected;

        playerData = PersistantData.GetPlayerData();
        levelObjectiveDatas = playerData.GetLevelObjectives(levelName);

        if (levelName.Equals("Level0"))
        {
            return;
        }

        StartCoroutine(ShowGameResults());
    }


    private IEnumerator ShowGameResults()
    {
        float score = Game.Score;
        float tempScore = 0;

     

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

        foreach (LevelObjectivesElement item in levelObjectives)
        {
            item.gameObject.SetActive(false);
        }

        for (int i = 0; i < levelObjectiveDatas.Length; i++)
        {
            levelObjectives[i].SetLevelObjective(levelObjectiveDatas[i]);
        }

        for (int i = 0; i < levelObjectives.Length; i++)
        {
            levelObjectives[i].gameObject.SetActive(true);

            levelObjectives[i].RefreshLevelObjectiveEement();

            levelObjectives[i].CheckComplete();

            yield return new WaitForSeconds(1.0f);
        }

        VideoRewardAd.SetActive(true);
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