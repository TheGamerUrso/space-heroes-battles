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
        float score = Game.Score;
        string scoreText = string.Format("{00:0000000000}", score);
        m_Text.text = scoreText;

        StartCoroutine(ScoreCoroutine());

        highscore.SetActive(false);
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
        float score = Game.Score;
        float tempScore = 0;
        string scoreText;

        yield return new WaitForSeconds(.2f);

        while (tempScore < score)
        {
            tempScore = Mathf.Lerp(tempScore, score, .5f);
            scoreText = string.Format("{00:0000000000}", tempScore);
            m_Text.text = scoreText;
            if (skip)
            {
                tempScore = score;
            }
            yield return null;
        }

        yield return new WaitForSeconds(1);

        if (Game.IsHightScore)
        {
            highscore.SetActive(true);
        }

        Game.IsHightScore = false;
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
