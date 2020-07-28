using System.Collections;
using System.Collections.Generic;
using EasyMobile;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SurvivalGameOverScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_Text;

    public GameObject highscore;
    private Level level;

    public void ShowResults()
    {
        Scene scene = SceneManager.GetActiveScene();
        level = GameManager.Instance.GetMission((LevelEnum)scene.buildIndex);

        PlayerData playerData = PersistantData.GetPlayerData();

        string scoreText = string.Format("{00:0000000000}", Game.Score);
        m_Text.text = scoreText;

        highscore.SetActive(false);
        int actualLevelIndex = level.mission.ID - (int)LevelEnum.Level0;

        var score = playerData.GetScore(actualLevelIndex);
        var hscore = playerData.GetHighScore(actualLevelIndex);

        if (Game.IsHightScore)
        {
            highscore.SetActive(true);
        }

        Game.IsHightScore = false;

        Game.IsSurvivalMode = false;
    }

    public void QuitButton()
    {
        GameManager.Instance.LoadMainenu();
    }

    public void ReplayButton()
    {
        GameManager.Instance.ResetLevel();
    }

}
