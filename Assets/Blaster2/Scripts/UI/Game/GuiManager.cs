using System.Collections;
using TMPro;
using UnityEngine;
using static GameController;

public class GuiManager : MonoBehaviour
{
    [Header("Menu")]

    [SerializeField] private UIView GameOverScreen;

    [SerializeField] private UIView WinScreen = null;

    [SerializeField] private UIView PauseScreen;

    [SerializeField] private RewardWidget rewardWidgetPanel;

    [SerializeField] private GameObject pauseButton;

    [SerializeField] private TextMeshProUGUI ScoreText;
    [SerializeField] private ScoreMultplierWidget scoreMultplierWidget;

    [SerializeField] private TransmitionWidget transmittionWidget;
    private float timer;

    public GameController gameController;

    //=================================================================================
    protected void OnDestroy()
    {
        
    }

    //=================================================================================
    protected void Awake()
    {
        transmittionWidget = FindObjectOfType<TransmitionWidget>();
    }
    //=================================================================================
    private void Start()
    {
        timer = 1;

        UpdateScore(0);
    }
    //=================================================================================
    private void Update()
    {
        if (gameController.CurrentGameState == GameController.GameState.GAME)
        {
#if UNITY_ANDROID
            if (!GameController.Instance.SlowMo)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    pauseButton.SetActive(false);
                }
            }
            else
            {
                timer = .25f;
                pauseButton.SetActive(true);
            }
#endif
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
            pauseButton.SetActive(true);
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                PauseButton();
            }
#endif
        }
    }
    //=================================================================================
    public void ShowRewardScreen()
    {
        rewardWidgetPanel.Show();
    }
    //=================================================================================
    public void ReplayButton()
    {
     
    }
    //=================================================================================
    public void ResumeButton()
    {

    }
    //=================================================================================
    public void PauseButton()
    {
     
    }
    //=================================================================================
    public void ShowPauseMenu(bool value)
    {
        if (value)
        {
            GameController.Instance.SlowMo = false;
            PauseScreen.Show();
        }
        else if (!value)
        {
            GameController.Instance.SlowMo = true;
            PauseScreen.Hide();
        }
    }
    //=================================================================================
    public void UpdateScore(int score)
    {
        string scoreText = string.Format("{00:00000000}", score);
        ScoreText.text = scoreText;
    }
    //=================================================================================
    public static void SetScoreMultipler(string text)
    {
      //  Instance.scoreMultplierWidget.SetText(text);
    }
    //=================================================================================
    public static void CreateFloatingText(string text, Vector3 pos)
    {
        GameObject m_floatingTextScript = PoolManager.Instance.GetObjectFromPool(PoolGameObjectType.FloatingText);
        m_floatingTextScript.SetActive(true);

        m_floatingTextScript.GetComponent<FloatingText>().ShowFloatingText(text, pos);
    }
    //=================================================================================
    public void QuitGameButton()
    {
        LoadMainMenu();
    }
    //=================================================================================
    public void LoadMainMenu()
    {
        GameController.Instance.IsGameOver = true;

        UIView activeMenuGO = null;

        if (WinScreen.IsActive())
        {
            activeMenuGO = WinScreen;
        }
        else if (GameOverScreen.IsActive())
        {
            activeMenuGO = GameOverScreen;
        }
        else if (PauseScreen.IsActive())
        {
            activeMenuGO = PauseScreen;
        }

        if (activeMenuGO != null)
            StartCoroutine(DelayCloseMenu(activeMenuGO, 1));
    }

    //=================================================================================
    IEnumerator DelayCloseMenu(UIView Menu, float time)
    {
        WaitForSeconds delay = new WaitForSeconds(time);
        yield return delay;
        Menu.Hide();
    }
    //=================================================================================
    public static void PlayTrasmition(string[] transmitions, bool playIntro = true)
    {
       // Instance.ShowTrasmition(transmitions, playIntro);
    }
    //=================================================================================
    public void BossWarning()
    {
        transmittionWidget.BossWarning();
    }
    //=================================================================================
    public void ShowTrasmition(string[] transmitions, bool playIntro = true)
    {
        if (transmittionWidget)
            transmittionWidget.RecieveTransmition(transmitions, playIntro);
    }
    //=================================================================================
    public bool IsTrasnmiting()
    {
        if (transmittionWidget != null)
        {
            return transmittionWidget.IncomingTransmition;
        }
        else
        {
            return false;
        }
    }

    //=================================================================================
    public void GameOver(bool IsPlayerAlive = false)
    {
        ShowPauseMenu(false);
        if (IsPlayerAlive)
        {
            WinScreen.Show();
            WinScreen.GetComponent<WinScreen>().ShowGameResult();
        }
        else
        {
            GameOverScreen.Show();      
        }
    }

}