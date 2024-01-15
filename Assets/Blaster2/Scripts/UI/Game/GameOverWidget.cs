using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverWidget : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_Text;
    private bool skip;
    private int retried = 0;

    private void OnEnable()
    {
        ShowGameResult();
    }

    public void ShowGameResult()
    {
        Scene scene = SceneManager.GetActiveScene();
        var playerData = PersistantData.GetPlayerData();
        float score = GameController.Instance.Score;
        string scoreText = string.Format("{00:00000000}", score);
        m_Text.text = scoreText;

        StartCoroutine(ScoreCoroutine());


    }
    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            skip = true;
        }
    }

    private IEnumerator ScoreCoroutine()
    {    
        var playerData = PersistantData.GetPlayerData();
        float score = GameController.Instance.Score;
        float tempScore = 0;
        string scoreText;

        yield return new WaitForSeconds(.2f);

        while (tempScore < score)
        {
            tempScore = Mathf.Lerp(tempScore, score, .5f);
            scoreText = string.Format("{00:00000000}", tempScore);
            m_Text.text = scoreText;
            if (skip)
            {
                tempScore = score;
            }
            yield return null;
        }
    }

    public void ReplayButton()
    {

        if (PlayerPrefs.HasKey("SurvivalAd"))
        {
            retried = PlayerPrefs.GetInt("SurvivalAd");
            retried++;
        }

          GameManager.Instance.ResetLevel();
  
    }

    public void LoadMainMenu()
    {
        GuiManager.Instance.LoadMainMenu();
    }
}