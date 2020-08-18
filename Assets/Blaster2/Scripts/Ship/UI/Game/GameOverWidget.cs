using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverWidget : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_Text;
    private Level level;

    private void OnEnable()
    {
        ShowGameResult();
    }

    public void ShowGameResult()
    {
        Scene scene = SceneManager.GetActiveScene();

        float score = Game.Score;
        string scoreText = string.Format("{00:0000000000}", score);
        m_Text.text = scoreText;

        level = GameManager.Instance.GetMission((LevelEnum)scene.buildIndex);
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