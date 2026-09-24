using System;
using System.Collections;
using TheGamerUrso.Core;
using TMPro;
using UnityEditor.MPE;
using UnityEngine;


public class GuiManager : MonoBehaviour
{
    public bool IncomingTransmition { get; set; }

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
    public GameMode gameMode;
    private IEventService eventService;

    //=================================================================================
    private void Start()
    {
        eventService = GameContext.Get<IEventService>();
        timer = 1;

        UpdateScore(0);
    }
    public void GameOver()
    {
        ShowPauseMenu(false);
        GameOverScreen.Show();
    }
    //=================================================================================
    public void Win()
    {
        ShowPauseMenu(false);
        WinScreen.Show();
        WinScreen.GetComponent<WinScreen>().ShowGameResult();
    }
    //=================================================================================
    private void Update()
    {
        if (gameController.CurrentGameState == GameState.GAME)
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
            GameController.Instance.IsSlowMo = false;
            PauseScreen.Show();
        }
        else if (!value)
        {
            GameController.Instance.IsSlowMo = true;
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
    public void SetScoreMultipler(string text)
    {
      scoreMultplierWidget.SetText(text);
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
    public void RecieveTransmition(string[] transmitions, 
        bool playIntro = true)
    {
        IncomingTransmition = true;
        if (transmittionWidget)
            transmittionWidget.RecieveTransmition(transmitions, playIntro, OnIncomingTranmsionEnded);
        eventService?.Publish(new IncomingTransmitionEvent());
    }
    //=================================================================================
    public void BossWarning()
    {
        IncomingTransmition = true;
        transmittionWidget.BossWarning(OnIncomingTranmsionEnded);
    }
    //=================================================================================
    public void OnIncomingTranmsionEnded()
    {
        IncomingTransmition = false;
    }
}