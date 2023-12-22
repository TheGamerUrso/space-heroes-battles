using System.Collections;
using TMPro;
using UnityEngine;
using Doozy.Engine.UI;
using static GameController;

public class GuiManager : MonoSingleton<GuiManager>
{
    [Header("Menu")]
    [SerializeField] private GameObject[] UIPrefabs;

    private UIView GameOverScreen;
    private UIView WinScreen = null;
    private UIView PauseScreen;

    private RewardWidget rewardWidgetPanel;

    [SerializeField] private GameObject pauseButton;

    [SerializeField] private TextMeshProUGUI ScoreText;
    [SerializeField] private ScoreMultplierWidget scoreMultplierWidget;

    [SerializeField]private TransmitionWidget transmittionWidget;
    private float timer;

    public GameObject WarningSign;


    protected override void OnCleanup()
    {
        base.OnCleanup();

        Events.OnGameOver -= GameOver;
        Events.OnWin -= Win;

        Events.OnPauseGame -= ShowPauseMenu;
        Events.OnScoreValueChanged -= UpdateScore;
    }

    protected override void Awake()
    {
        base.Awake();
        transmittionWidget = FindObjectOfType<TransmitionWidget>();
    }

    private void Start()
    {
        Events.OnScoreValueChanged += UpdateScore;
        Events.OnGameOver += GameOver;
        Events.OnWin += Win;
        Events.OnPauseGame += ShowPauseMenu;

        timer = 1;
        var panelPause = Instantiate(UIPrefabs[1], transform, false);
        var panelGameOver = Instantiate(UIPrefabs[2], transform, false);
        var panelWin = Instantiate(UIPrefabs[3], transform, false);
        var rewardWidget = Instantiate(UIPrefabs[4], transform, false);

        panelGameOver.name = UIPrefabs[2].name;
        panelWin.name = UIPrefabs[3].name;
        panelPause.name = UIPrefabs[1].name;
        rewardWidget.name = UIPrefabs[4].name;

        GameOverScreen = panelGameOver.GetComponent<UIView>();
        WinScreen = panelWin.GetComponent<UIView>();
        PauseScreen = panelPause.GetComponent<UIView>();

        rewardWidgetPanel = rewardWidget.GetComponent<RewardWidget>();

        UpdateScore(0);
    }

    private void Update()
    {
        if (GameController.CurrentGameState == GameController.GameState.GAME)
        {
            if (Time.timeScale == 1)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    pauseButton.SetActive(false);
                }
            }
            else
            {
                timer = 1;
                pauseButton.SetActive(true);
            }
        }
    }

    public void ShowRewardScreen()
    {
        rewardWidgetPanel.gameObject.SetActive(true);
        rewardWidgetPanel.GetNewRewards();
    }

    public void ReplayButton()
    {
        GameManager.Instance.ResetLevel();
    }

    public void ResumeButton()
    {
        Events.ToggleSlowMo?.Invoke(true);
        GameManager.Instance.PauseTheGame(false);
    }

    public void PauseButton()
    {
        Events.ToggleSlowMo?.Invoke(false);
        GameManager.Instance.PauseTheGame(true);
    }

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

    public void UpdateScore(int score)
    {
        string scoreText = string.Format("{00:00000000}", score);
        ScoreText.text = scoreText;
    }

    public static void SetScoreMultipler(string text)
    {
        Instance.scoreMultplierWidget.SetText(text);
    }
    public static void CreateFloatingText(string text, Vector3 pos)
    {
        GameObject m_floatingTextScript = PoolManager.Instance.GetObjectFromPool(PoolGameObjectType.FloatingText);
        m_floatingTextScript.SetActive(true);

        m_floatingTextScript.GetComponent<FloatingText>().ShowFloatingText(text, pos);
    }

    public void QuitGameButton()
    {
        LoadMainMenu();
    }

    public void LoadMainMenu()
    {
        GameController.Instance.IsGameOver = true; 

        GameManager.Instance.LoadMainenu();

        UIView activeMenuGO = null;

        if (WinScreen.IsActive())
        {
            activeMenuGO = WinScreen;
        }
        else if (GameOverScreen.IsActive())
        {
            activeMenuGO = GameOverScreen;
        }

        Events.ToggleSlowMo?.Invoke(false);
        GameManager.Instance.PauseTheGame(false);

        //else if (PauseScreen.IsActive())
        //{
        //    activeMenuGO = PauseScreen;   
        //}

        if (activeMenuGO != null)
            StartCoroutine(DelayCloseMenu(activeMenuGO, 1));
    }

    IEnumerator DelayCloseMenu(UIView Menu, float time)
    {
        WaitForSeconds delay = new WaitForSeconds(time);
        yield return delay;
        Menu.Hide();
    }

    public static void PlayTrasmition(string[] transmitions)
    {
        Instance.ShowTrasmition(transmitions);
    }

    public void BossWarning()
    {
        transmittionWidget.BossWarning();
    }

    public void ShowTrasmition(string[] transmitions)
    {
        if (transmittionWidget)
            transmittionWidget.RecieveTransmition(transmitions);
    }

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

    public void Win(BaseGameMode baseGameMode)
    {
        WinScreen.Show();
        WinScreen.GetComponent
            <WinScreen>().ShowGameResult();
    }
    public void GameOver(BaseGameMode baseGameMode)
    {
        GameOverScreen.Show();
    }

}