using System.Collections;
using TheGamerUrso.Core;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverWidget : UIView
{
    [SerializeField] private TextMeshProUGUI m_Text;
    private bool skip;
    private int retried = 0;

    protected IDataService dataService;
    private IAppService appService;


    public void ShowGameResult()
    {
        Scene scene = SceneManager.GetActiveScene();
        var playerData = dataService.GetPlayerData();
        float score = GameController.Instance.Score;
        string scoreText = string.Format("{00:00000000}", score);
        m_Text.text = scoreText;

        StartCoroutine(ScoreCoroutine());
    }

    private void Awake()
    {
      
    }

    private void Start()
    {
        dataService = GameContext.Get<IDataService>();
        appService = GameContext.Get<IAppService>();
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

        appService.ResetLevel();
        GetComponent<CanvasGroup>().interactable = false;

    }
    public void QuitButton()
    {
        appService.LoadMainMenu();
        GetComponent<CanvasGroup>().interactable = false;
    }
}