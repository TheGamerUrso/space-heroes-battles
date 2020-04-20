using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using TheGamerUrso.SceneLoader;

public class WinWidget : MonoBehaviour
{
    private PlayerData playerData;

    [SerializeField] private TextMeshProUGUI Score = null;
    private string levelName;

    [Header("Win Widget Config")]
    public LevelObjectivesElement[] levelObjectives;
    private LevelObjectiveData[] levelObjectiveDatas;

    public void ShowGameResult()
    {
        int levelIndex = GameManager.LevelIndexSelected;
        int levelSelected = (levelIndex + 1);
        var levelName = "Level" + levelSelected;

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
        float score = GameSession.score;
        string scoreText = string.Format("{00:0000000000}", score);
        Score.text = scoreText;



        int ChallengeIndex = 0;

        foreach (LevelObjectivesElement item in levelObjectives)
        {
            item.gameObject.SetActive(false);
        }

        for (int i = 0; i < levelObjectives.Length; i++)
        {
            levelObjectives[i].levelObjectiveData = levelObjectiveDatas[i];
        }

        yield return new WaitForSeconds(1.0f);

        levelObjectives[ChallengeIndex].gameObject.SetActive(true);

        levelObjectives[ChallengeIndex].RefreshLevelObjectiveEement();

        levelObjectives[ChallengeIndex].CheckComplete();

        ChallengeIndex = 1;

        yield return new WaitForSeconds(1.0f);

        levelObjectives[ChallengeIndex].gameObject.SetActive(true);

        levelObjectives[ChallengeIndex].RefreshLevelObjectiveEement();

        levelObjectives[ChallengeIndex].CheckComplete();

        ChallengeIndex = 2;

        yield return new WaitForSeconds(1.0f);

        levelObjectives[ChallengeIndex].gameObject.SetActive(true);

        levelObjectives[ChallengeIndex].RefreshLevelObjectiveEement();

        levelObjectives[ChallengeIndex].CheckComplete();

        ChallengeIndex = 3;

        yield return new WaitForSeconds(1.0f);

        levelObjectives[ChallengeIndex].gameObject.SetActive(true);

        levelObjectives[ChallengeIndex].RefreshLevelObjectiveEement();

        levelObjectives[ChallengeIndex].CheckComplete();
    }

    public void ReplayButton()
    {
        SceneLoader.Instance.ResetLevel();
    }
    public void LoadMainMenu()
    {
        GuiManager.Instance.LoadMainMenu();
    }
}