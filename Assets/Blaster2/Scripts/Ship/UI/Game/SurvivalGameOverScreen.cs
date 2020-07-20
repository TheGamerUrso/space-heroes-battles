using System.Collections;
using System.Collections.Generic;
using EasyMobile;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SurvivalGameOverScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_Text;

    public GameObject highscore;

    public void ShowResults()
    {
        PlayerData playerData = PersistantData.GetPlayerData();

        string scoreText = string.Format("{00:0000000000}", Game.Score);
        m_Text.text = scoreText;

        highscore.SetActive(false);

        var score = playerData.GetScore(GameManager.LevelIndexSelected);
        var hscore = playerData.GetHighScore(GameManager.LevelIndexSelected);

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
