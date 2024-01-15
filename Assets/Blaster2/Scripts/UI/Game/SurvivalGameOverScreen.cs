using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SurvivalGameOverScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_Text;

    public GameObject highscore;

    private bool skip;
    private int retried = 0;

    public void ShowResults()
    {
        var playerData = PersistantData.GetPlayerData();
        float score = GameController.Instance.Score;
        string scoreText = string.Format("{00:00000000}", score);
        m_Text.text = scoreText;

        StartCoroutine(ScoreCoroutine());
        
        highscore.SetActive(false);
    }

    private IEnumerator ScoreCoroutine()
    {            
        GetComponent<CanvasGroup>().interactable = false;
        var playerData = PersistantData.GetPlayerData();
        int score = GameController.Instance.Score;
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

        yield return new WaitForSeconds(1);

        if (GameController.Instance.IsHightScore)
        {
            highscore.SetActive(true);
        }

        GameController.Instance.IsHightScore = false;
        GetComponent<CanvasGroup>().interactable = true;

        TheGamerUrso.Leaderboards.Instance.LoadLeaderboard();
    }

    public void QuitButton()
    {
        GameManager.Instance.LoadMainenu();
        GetComponent<CanvasGroup>().interactable = false;
    }

    public void ReplayButton()
    {
        GameManager.Instance.ResetLevel();
        GetComponent<CanvasGroup>().interactable = false;
    }
}
