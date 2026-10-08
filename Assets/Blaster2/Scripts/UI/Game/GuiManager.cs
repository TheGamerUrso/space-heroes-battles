using System;
using System.Collections;
using System.Runtime.CompilerServices;
using TheGamerUrso.Core;
using TMPro;
using UnityEditor;
using UnityEditor.MPE;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;


public class GuiManager : ServiceComponent<IGUIService>, IGUIService
{
    public bool IsMenuOpen { get; }
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

    [SerializeField] private GameObject PlayerHud;
    [SerializeField] private GameObject CoinsUI;

    [SerializeField] private PlayerXPUI playerXP;
    [SerializeField] private PlayerPowerCircleUI powerCircleUI;
    [SerializeField] private PlayerHealthUI playerHealthUI;
    [SerializeField] private LowHealthIndicator lowHealthIndicator;
    [SerializeField] private WarningSignUI warningSignUI;

    private IEventService eventService;
    private IAppService appService;
    protected override void Awake()
    {
        base.Awake();
        PlayerHud.SetActive(false);
        CoinsUI.SetActive(false);
    }

    //=================================================================================
    private void Start()
    {
        appService = GameContext.Get<IAppService>();
        eventService = GameContext.Get<IEventService>();
        timer = 1;
        eventService.Subscribe<FloatingTextEvent>(CreateFloatingText);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        eventService.Unsubscribe<FloatingTextEvent>(CreateFloatingText);
    }

    public void Setup(PlayerShip ship,PlayerData playerData)
    {
        playerXP.Setup(playerData.GetCurrentPlayerShipData());
        playerHealthUI.Setup(ship.GetComponent<IDamagable>());
        powerCircleUI.Setup(ship);
        lowHealthIndicator.Setup(ship);
        warningSignUI.Setup(ship.gameObject);

        PlayerHud.SetActive(true);
        CoinsUI.SetActive(true);
    }

    public void GameOver()
    {
        GameOverScreenUI.Show();
    }
    //=================================================================================
    public void Win()
    {
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
        }
    }
    //=================================================================================
    public void ShowRewardScreen()
    {
        rewardWidgetPanel.Show();
    }
    //=================================================================================
    public void PauseButton()
    {
        if (appService.CurrentGameState == TheGamerUrso.Core.GameStateEnum.PAUSED)
        {
            appService.Unpause();
            gameController.IsSlowMo = false;
            PauseScreenUI.Hide();
           
        }
        else if (appService.CurrentGameState == TheGamerUrso.Core.GameStateEnum.GAME)
        {
            appService.Pause();
            gameController.IsSlowMo = true;
            PauseScreenUI.Show();
        }
    }
    //=================================================================================
    public void CreateFloatingText(FloatingTextEvent payload)
    {
        GameObject m_floatingTextScript = PoolManager.Instance.GetObjectFromPool(PoolGameObjectType.FloatingText);
        m_floatingTextScript.SetActive(true);

        m_floatingTextScript.GetComponent<FloatingText>().ShowFloatingText(payload.Message, payload.targetPos);
    }
    //=================================================================================
    public void QuitButton()
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

        appService.Unpause();
        activeMenuGO.Hide();
        appService.LoadMainMenu();   
    }
    //=================================================================================
    IEnumerator DelayCloseMenu(UIView Menu, float time)
    {
        appService.Unpause();
        WaitForSeconds delay = new WaitForSeconds(time);
        yield return delay;
        Menu.Hide();
        appService.LoadMainMenu();
    }
    //=================================================================================
    public void RecieveTransmition(string[] transmitions = null, bool playIntro = true)
    {
        IncomingTransmition = true;
        if (incomingMessageUI)
        {
            ((IncomingMessageUI)incomingMessageUI).RecieveTransmition(
                transmitions,
                playIntro,
                OnIncomingTranmsionEnded);
        }
        eventService?.Publish(new IncomingTransmitionEvent());
    }   
    //=================================================================================
    public void BossWarning(Action callback)
    {
        IncomingTransmition = true;
        IncomingBossUI.RecieveTransmition(new string[0],
            false, () => 
            {
                callback?.Invoke();
                OnIncomingTranmsionEnded();
            });
    }
    //=================================================================================
    public void OnIncomingTranmsionEnded()
    {
        IncomingTransmition = false;
    }
}