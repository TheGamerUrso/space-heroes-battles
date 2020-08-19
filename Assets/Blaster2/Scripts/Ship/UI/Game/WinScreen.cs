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
        Scene scene = SceneManager.GetActiveScene();
        levelName = ((LevelEnum)scene.buildIndex).ToString();

        levelObjectiveDatas = PersistantData.GetLevelObjectives(levelName);

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

        for (int i = 0; i < levelObjectiveDatas.Length; i++)
        {
            levelObjectives[i].SetLevelObjective(levelObjectiveDatas[i]);
        }

        for (int i = 0; i < levelObjectiveDatas.Length; i++)
        {
            levelObjectives[i].gameObject.SetActive(true);
            levelObjectives[i].CheckComplete();
            yield return new WaitForSeconds(.5f);
        }

        VideoRewardAd.SetActive(true);
    }


    public void NextLevelButton()
    {
        Scene scene = SceneManager.GetActiveScene();
        int nextLevelID = (scene.buildIndex + 1) - (int)LevelEnum.Level0;

        if (nextLevelID > 9)
        {
            nextLevelID = 9;
        }

        Level nextLevel = PersistantData.GetLevels()[nextLevelID];

        GameManager.Instance.SetMission(nextLevel);
        GameManager.Instance.LoadScene((LevelEnum)nextLevel.mission.ID);
    }

    public void ReplayButton()
    {
        Scene scene = SceneManager.GetActiveScene();
        GameManager.Instance.LoadScene((LevelEnum)scene.buildIndex);
    }

    public void LoadMainMenu()
    {
        GuiManager.Instance.LoadMainMenu();
    }
}