using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using EasyMobile;
using TheGamerUrso.PoolSystem;
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
        int score = GameSession.Multiplier * baseEnemy.m_ValueOfEnemy;
        GuiManager.CreateFloatingText(score.ToString(), baseEnemy.transform.position);
    }

    public void PickUpItem(ItemData itemData)
    {
        if (itemData.m_HealValue > 0)
        {
            CreateFloatingText("Heal up", transform.localPosition);

            if (!PlayerPrefs.HasKey("HealTut"))
            {
                if (Tutorial.Instance)
                {
                    Tutorial.Instance.ShowTutorial(1);
                }
                PlayerPrefs.SetInt("HealTut", 1);
            }
        }

        if (itemData.m_RewardAmount > 0)
        {
            CreateFloatingText("$", transform.localPosition);

            if (!PlayerPrefs.HasKey("CoinTut"))
            {
                if (Tutorial.Instance)
                {
                    Tutorial.Instance.ShowTutorial(0);
                }

                itemData.ShowTutorial = true;

                PlayerPrefs.SetInt("CoinTut", 1);
            }
        }

        if (itemData.Shield)
        {
            CreateFloatingText("Shield Up", transform.localPosition);

            if (!PlayerPrefs.HasKey("ShieldTut"))
            {

                if (Tutorial.Instance)
                {
                    Tutorial.Instance.ShowTutorial(3);
                }

                itemData.ShowTutorial = true;

                PlayerPrefs.SetInt("ShieldTut", 1);
            }
        }

        if (itemData.PowerPack)
        {
            CreateFloatingText("Power Up", transform.localPosition);


            if (!PlayerPrefs.HasKey("PowerTut"))
            {
                if (Tutorial.Instance)
                {
                    Tutorial.Instance.ShowTutorial(2);
                }

                itemData.ShowTutorial = true;

                PlayerPrefs.SetInt("PowerTut", 1);
            }
        }
    }

    private void Update()
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
            GameSession.useSloMo = false;
            PauseScreen.Show();
        }
        else if (!value)
        {
            GameSession.useSloMo = true;
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
        GameSession.SurvivalMode = false;
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
        if (GameOverScreen.GetComponent<GameOverWidget>() != null)
            GameOverScreen.GetComponent<GameOverWidget>().ShowGameResult();
    }

    public IEnumerator WinCoroutine()
    {
        AudioManager.PlayMusic("Victory", false);

        yield return new WaitForSeconds(2.0f);

        WinScreen.Show();
        WinScreen.GetComponent
            <WinWidget>().ShowGameResult();
    }
}