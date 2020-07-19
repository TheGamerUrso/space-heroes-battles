using EasyMobile;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverWidget : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_Text;
    private void OnEnable()
    {
        ShowGameResult();
    }

    public void ShowGameResult()
    {
        float score = Game.Score;
        string scoreText = string.Format("{00:0000000000}", score);
        m_Text.text = scoreText;

        string levelName = "Level" + GameManager.LevelIndexSelected;

        if (levelName.Equals("Level0"))
        {
            return;
        }
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