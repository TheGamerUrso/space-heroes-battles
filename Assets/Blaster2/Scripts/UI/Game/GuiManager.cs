using System;
using System.Collections;
using System.Runtime.CompilerServices;
using TheGamerUrso.Core;
using TMPro;
using UnityEditor.MPE;
using UnityEngine;


public class GuiManager : MonoBehaviour
{
    public bool IncomingTransmition { get; set; }

    [Header("Menu")]

    [SerializeField] private UIView GameOverScreenUI;

    [SerializeField] private UIView WinScreenUI = null;

    [SerializeField] private UIView PauseScreenUI;

    [SerializeField] private RewardUI rewardWidgetPanel;

    [SerializeField] private GameObject pauseButton;

    [SerializeField] private TextMeshProUGUI ScoreText;
    [SerializeField] private ScoreMultplierWidget scoreMultplierWidget;

    [SerializeField] private BaseIncomingMessage incomingMessageUI;
    [SerializeField] private BaseIncomingMessage IncomingBossUI;
    private float timer;

    public GameController gameController;
    public WaveManager waveManager;

    [SerializeField] private PlayerXPUI playerXP;
    [SerializeField] private PlayerPowerCircleUI powerCircleUI;
    [SerializeField] private PlayerHealthUI playerHealthUI;
    [SerializeField] private LowHealthIndicator lowHealthIndicator;
    [SerializeField] private WarningSignUI warningSignUI;

    private IEventService eventService;

    //=================================================================================
    private void Start()
    {
        eventService = GameContext.Get<IEventService>();
        timer = 1;

        UpdateScore(0);
    }

    public void Setup(PlayerShip ship,PlayerData playerData)
    {
        playerXP.Setup(playerData.GetCurrentPlayerShipData());
        playerHealthUI.Setup(ship.GetComponent<IDamagable>());
        powerCircleUI.Setup(playerData, playerData.GetCurrentPlayerShipData());
        lowHealthIndicator.Setup(ship);
        warningSignUI.Setup(ship.gameObject);

        eventService.Subscribe<EnemyDiedEvent>(EnemyDiedHandled);
    }


    public void EnemyDiedHandled(EnemyDiedEvent enemyDied)
    {

    }

    public void GameOver()
    {
        ShowPauseMenu(false);
        GameOverScreenUI.Show();
    }
    //=================================================================================
    public void Win()
    {
        ShowPauseMenu(false);
        WinScreenUI.Show();
        WinScreenUI.GetComponent<WinScreenUI>().ShowGameResult(gameController.Score);
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
            gameController.IsSlowMo = false;
            PauseScreenUI.Show();
        }
        else if (!value)
        {
            gameController.IsSlowMo = true;
            PauseScreenUI.Hide();
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
        UIView activeMenuGO = null;

        if (WinScreenUI.IsActive)
        {
            activeMenuGO = WinScreenUI;
        }
        else if (GameOverScreenUI.IsActive)
        {
            activeMenuGO = GameOverScreenUI;
        }
        else if (PauseScreenUI.IsActive)
        {
            activeMenuGO = PauseScreenUI;
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
        if (incomingMessageUI)
        {
            ((IncomingMessageUI)incomingMessageUI).RecieveTransmition(transmitions,
                playIntro, OnIncomingTranmsionEnded);
        }
        eventService?.Publish(new IncomingTransmitionEvent());
    }
    //=================================================================================
    public void BossWarning()
    {
        IncomingTransmition = true;
        IncomingBossUI.RecieveTransmition(OnIncomingTranmsionEnded);
    }
    //=================================================================================
    public void OnIncomingTranmsionEnded()
    {
        IncomingTransmition = false;
    }
}