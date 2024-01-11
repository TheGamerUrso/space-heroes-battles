using System.Collections;
using TMPro;
using UnityEngine;
using Doozy.Engine.UI;
using static GameController;

public class GuiManager : MonoSingleton<GuiManager>
{
    [Header("Menu")]
    
    [SerializeField] private UIView GameOverScreen;
    
    [SerializeField] private UIView WinScreen = null;
    
    [SerializeField] private UIView PauseScreen;
    
    [SerializeField] private RewardWidget rewardWidgetPanel;

    [SerializeField] private GameObject pauseButton;

    [SerializeField] private TextMeshProUGUI ScoreText;
    [SerializeField] private ScoreMultplierWidget scoreMultplierWidget;

    [SerializeField]private TransmitionWidget transmittionWidget;
    private float timer;

//=================================================================================
    protected override void OnCleanup()
    {
        base.OnCleanup();

        Events.OnGameOver -= GameOver;
        Events.OnPauseGame -= ShowPauseMenu;
        Events.OnScoreValueChanged -= UpdateScore;
    }
//=================================================================================
    protected override void Awake()
    {
        base.Awake();
        transmittionWidget = FindObjectOfType<TransmitionWidget>();
    }
//=================================================================================
    private void Start()
    {
        Events.OnScoreValueChanged += UpdateScore;
        Events.OnGameOver += GameOver;
        Events.OnPauseGame += ShowPauseMenu;

        timer = 1;

        UpdateScore(0);
    }
//=================================================================================
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

            if(Input.GetKeyDown(KeyCode.Escape)){
                PauseButton();
            }

        }
    }
//=================================================================================
    public void ShowRewardScreen()
    {
        rewardWidgetPanel.gameObject.SetActive(true);
        rewardWidgetPanel.GetNewRewards();
    }
//=================================================================================
    public void ReplayButton()
    {
        GameManager.Instance.ResetLevel();
    }
//=================================================================================
    public void ResumeButton()
    {
        Events.ToggleSlowMo?.Invoke(true);
        GameManager.Instance.PauseTheGame(false);
    }
//=================================================================================
    public void PauseButton()
    {
        Events.ToggleSlowMo?.Invoke(false);
        GameManager.Instance.PauseTheGame(true);
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
        Instance.scoreMultplierWidget.SetText(text);
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

        Events.ToggleSlowMo?.Invoke(false);
        GameManager.Instance.PauseTheGame(false);

        if (activeMenuGO != null)
            StartCoroutine(DelayCloseMenu(activeMenuGO, 1));


        GameManager.Instance.LoadMainenu();
    }

    //=================================================================================
    IEnumerator DelayCloseMenu(UIView Menu, float time)
    {
        WaitForSeconds delay = new WaitForSeconds(time);
        yield return delay;
        Menu.Hide();
    }
//=================================================================================
    public static void PlayTrasmition(string[] transmitions)
    {
        Instance.ShowTrasmition(transmitions);
    }
//=================================================================================
    public void BossWarning()
    {
        transmittionWidget.BossWarning();
    }
//=================================================================================
    public void ShowTrasmition(string[] transmitions)
    {
        if (transmittionWidget)
            transmittionWidget.RecieveTransmition(transmitions);
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
    public void GameOver(BaseGameMode baseGameMode, bool IsPlayerAlive = false)
    {
        if (IsPlayerAlive)
        {
            WinScreen.Show();
            WinScreen.GetComponent
                <WinScreen>().ShowGameResult();
        }
        else
        {
            GameOverScreen.Show();
        }

    }

}