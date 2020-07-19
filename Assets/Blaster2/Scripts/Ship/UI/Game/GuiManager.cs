using System.Collections;
using TMPro;
using UnityEngine;
using Doozy.Engine.UI;

public class GuiManager : MonoSingleton<GuiManager>
{
    [Header("Menu")]
    [SerializeField] private GameObject[] UIPrefabs;

    private UIView GameOverScreen;
    private UIView WinScreen = null;
    private UIView PauseScreen;

    [SerializeField] private GameObject pauseButton;

    [SerializeField] private TextMeshProUGUI ScoreText;
    [SerializeField] private TextMeshProUGUI CoinWidgetText;
    [SerializeField] private TextMeshProUGUI CountdownWidgetText;

    private TransmitionWidget transmittionWidget;
    private float timer;

    protected override void OnCleanup()
    {
        base.OnCleanup();

        Events.OnGameOver -= GameOver;
        Events.OnWin -= Win;

        Events.OnPauseGame -= ShowPauseMenu;
        Events.OnCoinValueChanged -= UpdateCoinWidgetText;
        Events.OnScoreValueChanged -= UpdateScore;
    }

    protected override void Awake()
    {
        base.Awake();
        transmittionWidget = FindObjectOfType<TransmitionWidget>();
    }

    private void Start()
    {
        timer = 1;

        var panelGameOver = Instantiate(UIPrefabs[0], transform, false);
        var panelWin = Instantiate(UIPrefabs[1], transform, false);
        var panelPause = Instantiate(UIPrefabs[2], transform, false);

        panelGameOver.name = UIPrefabs[0].name;
        panelWin.name = UIPrefabs[1].name;
        panelPause.name = UIPrefabs[2].name;

        GameOverScreen = panelGameOver.GetComponent<UIView>();
        WinScreen = panelWin.GetComponent<UIView>();
        PauseScreen = panelPause.GetComponent<UIView>();

        Events.OnCoinValueChanged += UpdateCoinWidgetText;
        Events.OnScoreValueChanged += UpdateScore;
        Events.OnGameOver += GameOver;
        Events.OnWin += Win;
        Events.EnemyDied += EnemyDiedCallback;
        Events.OnPauseGame += ShowPauseMenu;

        UpdateScore(0);
        UpdateCoinWidgetText(0);
    }

    public void EnemyDiedCallback(string name, BaseEnemy baseEnemy)
    {
        int score = Game.Multiplier * baseEnemy.EnemyData.EnemyValue;
        GuiManager.CreateFloatingText(score.ToString(), baseEnemy.transform.position);
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

    public void SetCountdownVisibility(bool enable)
    {
        CountdownWidgetText.gameObject.SetActive(enable);
    }

    public static void CountdownVisibility(bool enable)
    {
        Instance.SetCountdownVisibility(enable);
    }
    public static void Countdown(float countdown)
    {
        Instance.CountdownText(countdown);
    }

    public void CountdownText(float countdown)
    {
        CountdownWidgetText.text = Mathf.Round(countdown).ToString();
    }

    public void ReplayButton()
    {
        GameManager.Instance.ResetLevel();
    }

    public void ResumeButton()
    {
        GameEventSystem.Call(GameEventType.ToggleSlowMo, true);
        GameManager.Instance.PauseTheGame(false);
    }

    public void PauseButton()
    {
        GameEventSystem.Call(GameEventType.ToggleSlowMo, false);
        GameManager.Instance.PauseTheGame(true);
    }

    public void ShowPauseMenu(bool value)
    {
        if (value)
        {
            Game.SlowMo = false;
            PauseScreen.Show();
        }
        else if (!value)
        {
            Game.SlowMo = true;
            PauseScreen.Hide();
        }
    }


    public void UpdateCoinWidgetText(int coin)
    {
        CoinWidgetText.text = coin.ToString();
    }

    public void UpdateScore(int score)
    {
        string scoreText = string.Format("{00:00000000}", score);
        ScoreText.text = scoreText;
    }

    public static void CreateFloatingText(string text, Vector3 pos)
    {
        GameObject m_floatingTextScript = PoolManager.Instance.GetObjectFromPool(PoolGameObjectType.FloatingText);
        m_floatingTextScript.GetComponent<FloatingText>().ShowFloatingText(text, pos);
    }

    public void QuitGameButton()
    {
        LoadMainMenu();
    }

    public void LoadMainMenu()
    {
        Game.IsSurvivalMode = false;
        Time.timeScale = 1;
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
        else if (PauseScreen.IsActive())
        {
            activeMenuGO = PauseScreen;
        }

        if (activeMenuGO != null)
            StartCoroutine(DelayCloseMenu(activeMenuGO, 1));
    }

    IEnumerator DelayCloseMenu(UIView Menu, float time)
    {
        WaitForSeconds delay = new WaitForSeconds(time);
        yield return delay;
        Menu.Hide();
    }

    public static void PlayTrasmition(string[] transmitions, bool boss = false)
    {
        Instance.ShowTrasmition(transmitions, boss);
    }

    public void ShowTrasmition(string[] transmitions, bool boss = false)
    {
        if (transmittionWidget)
            transmittionWidget.RecieveTransmition(transmitions, boss);
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

    public void Win(GameController gc)
    {
        StartCoroutine(WinCoroutine());
    }
    public void GameOver(GameController gc)
    {
        StartCoroutine(GameOverCoroutine());
    }

    private IEnumerator GameOverCoroutine()
    {

        yield return new WaitForSeconds(2.0f);

        GameOverScreen.Show();
    }

    public IEnumerator WinCoroutine()
    {
        AudioManager.PlayMusic("Victory", false);

        yield return new WaitForSeconds(2.0f);

        WinScreen.Show();
        WinScreen.GetComponent
            <WinScreen>().ShowGameResult();
    }
}