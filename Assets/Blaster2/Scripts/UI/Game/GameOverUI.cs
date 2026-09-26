using System.Collections;
using TheGamerUrso.Core;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : UIView
{
    [SerializeField] private GameController gameController;
    [SerializeField] private TextMeshProUGUI m_Text;
    [SerializeField] private CanvasGroup canvasGroup;
    private bool skip;
    private int retried = 0;
    private float score;

    protected IDataService dataService;
    private IAppService appService;


    public void ShowGameResult(float score)
    {
        Scene scene = SceneManager.GetActiveScene();
        var playerData = dataService.GetPlayerData();
        this.score = score;
        m_Text.text = string.Format("{00:00000000}", 0); 
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
        var playerData = dataService.GetPlayerData();
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
        appService.ResetLevel();
        canvasGroup.interactable = false;
    }

    public void QuitButton()
    {
        appService.LoadMainMenu();
        canvasGroup.interactable = false;
    }
}