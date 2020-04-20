using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using EasyMobile;
using TheGamerUrso.PoolSystem;
using TheGamerUrso.SceneLoader;
using Doozy.Engine.UI;

public class GuiManager : Singleton<GuiManager>
{
    PlayerShip playerShip;

    [Header("Menu")]
    [SerializeField] private UIView GameOverScreen;
    [SerializeField] private UIView WinScreen = null;
    [SerializeField] private UIView PauseScreen;
    [SerializeField] private GameObject pauseButton;

    [Header("PlayerHUD")]
    [SerializeField] private GameObject PlayerHUD;
    [SerializeField] private GameObject SpecialIsReadyFeedback;
    [SerializeField] private TextMeshProUGUI ScoreText;
    [SerializeField] private TextMeshProUGUI CoinWidgetText;
    [SerializeField] private TextMeshProUGUI CountdownWidgetText;

    private bool ResultShowed = false;
    private TransmitionWidget transmittionWidget;
    private float timer;

    private void OnApplicationFocus(bool focus)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (!focus && GameSession.IsGameOver == false)
            {
                GameManager.Instance.PauseTheGame(focus);
            }
        }
    }

    private void OnApplicationPause(bool Paused)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (GameSession.IsGameOver == false)
            {
                GameManager.Instance.PauseTheGame(Paused);
            }
        }
    }

    protected override void OnCleanup()
    {
        base.OnCleanup();

        GameController.OnGameOver -= GameOver;
        GameController.OnWin -= Win;


        GameManager.Instance.OnPauseGame -= ShowPauseMenu;
        GameSession.OnCoinValueChanged -= UpdateCoinWidgetText;
        GameSession.OnScoreValueChanged -= UpdateScore;
    }

    protected override void OnAwake()
    {
        base.OnAwake();

        transmittionWidget = FindObjectOfType<TransmitionWidget>();
    }

    private void Start()
    {
        timer = 1;

        if (playerShip == null)
        {
            playerShip = PlayerManager.GetPlayer();
        }

        GameSession.OnCoinValueChanged += UpdateCoinWidgetText;
        GameSession.OnScoreValueChanged += UpdateScore;


        GameController.OnGameOver += GameOver;
        GameController.OnWin += Win;

        SpawnEnemies spawnEnemies = GameObject.FindObjectOfType<SpawnEnemies>();
        if (spawnEnemies != null)
        {
            spawnEnemies.EnemyDied += EnemyDiedCallback;
        }


        GameManager.Instance.OnPauseGame += ShowPauseMenu;
    }

    public void EnemyDiedCallback(BaseEnemy baseEnemy)
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

        UpdateScore(75);
    }

    private void Update()
    {
        //if (Time.timeScale == 1)
        //{
        //    timer -= Time.deltaTime;
        //    if (timer <= 0)
        //    {
        //        pauseButton.SetActive(false);
        //    }
        //}
        //else
        //{
        //    timer = 1;
        //    pauseButton.SetActive(true);
        //}
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
        SceneLoader.Instance.ResetLevel();
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
            PauseScreen.Show();
        }
        else if (!value)
        {
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
        SceneLoader.Instance.LoadMainenu();

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
        AudioManager.PlaySound(null, "transmition", 3);
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

        PlayerHUD.gameObject.SetActive(false);

        yield return new WaitForSeconds(2.0f);

        GameOverScreen.Show();
        GameOverScreen.GetComponent<GameOverWidget>().ShowGameResult();
    }

    public IEnumerator WinCoroutine()
    {
        AudioManager.PlayMusic("Victory", false);

        PlayerHUD.gameObject.SetActive(false);

        yield return new WaitForSeconds(2.0f);

        WinScreen.Show();
        WinScreen.GetComponent
            <WinWidget>().ShowGameResult();
    }
}