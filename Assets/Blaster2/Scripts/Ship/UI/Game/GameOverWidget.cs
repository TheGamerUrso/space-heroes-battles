using EasyMobile;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverWidget : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_Text;
    private Level level;
    private bool skip;
    private int retried = 0;

    private void OnEnable()
    {
        Advertising.InterstitialAdCompleted += InterstitialAdCompletedHandler;
        ShowGameResult();
    }

    public void ShowGameResult()
    {
        Scene scene = SceneManager.GetActiveScene();
        level = GameManager.Instance.GetMission((LevelEnum)scene.buildIndex);

        float score = Game.Score;
        string scoreText = string.Format("{00:0000000000}", score);
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
    }

    public void ReplayButton()
    {

        if (PlayerPrefs.HasKey("SurvivalAd"))
        {
            retried = PlayerPrefs.GetInt("SurvivalAd");
            retried++;
        }

        if (retried >= 5)
        {
            retried = 0;
            AdvertismentManager.ShowAdvertisment();
        }
        else
        {
            GameManager.Instance.ResetLevel();
        }
  
    }

    public void LoadMainMenu()
    {
        GuiManager.Instance.LoadMainMenu();
    }


    // The event handler
    void InterstitialAdCompletedHandler(InterstitialAdNetwork network, AdLocation location)
    {
        Debug.Log("Interstitial ad has been closed.");
        GameManager.Instance.ResetLevel();
        PlayerPrefs.SetInt("SurvivalAd", 0);
    }

    // Unsubscribe
    void OnDisable()
    {
        Advertising.InterstitialAdCompleted -= InterstitialAdCompletedHandler;
    }

}