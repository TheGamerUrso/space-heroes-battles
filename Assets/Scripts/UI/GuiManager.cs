using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using EasyMobile;
using TheGamerUrso.PoolSystem;

public class GuiManager : Singleton<GuiManager>
{
    PlayerShip playerShip;

    [Header("Menu")]
    [SerializeField] private GameObject GameOverScreen;
    [SerializeField] private GameObject WinScreen = null;
    [SerializeField] private GameObject PauseScreen;
    [SerializeField] private GameObject pauseButton;

    [Header("PlayerHUD")]
    [SerializeField] private GameObject PlayerHUD;
    [SerializeField] private GameObject SpecialIsReadyFeedback;
    [SerializeField] private TextMeshProUGUI ScoreText;
    [SerializeField] private TextMeshProUGUI CoinWidgetText;
    [SerializeField] private TextMeshProUGUI CountdownWidgetText;

    private bool ResultShowed = false;
    private float timer;

    private void OnApplicationFocus(bool focus)
    {
        //if (Application.platform == RuntimePlatform.Android)
        //{
        //    if (!focus && GameController.IsGameOver == false)
        //    {
        //        GameManager.PauseTheGame();
        //        ShowPauseMenu(true);
        //    }
        //}
    }

    private void OnApplicationPause(bool Paused)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            //TODO Update Pause
            //if (GameController.IsGameOver == false)
            //{
            //    GameManager.PauseTheGame();
            //    ShowPauseMenu(true);
            //}
        }
    }

    protected override void OnCleanup()
    {
        base.OnCleanup();
        if (playerShip != null)
        {
            playerShip.PickUpItem -= PickUpItem;
            GameEventSystem.PickUpEvent -= UpdateCoinWidgetText;
        }

        GameController.OnGameOver -= GameOver;
        GameController.OnWin -= Win;
    }

    private void Start()
    {
        timer = 1;

        if (playerShip == null)
        {
            playerShip = PlayerManager.GetPlayer();
        }

        playerShip.PickUpItem += PickUpItem;
        GameEventSystem.PickUpEvent += UpdateCoinWidgetText;

        GameController.OnGameOver += GameOver;
        GameController.OnWin += Win;
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
        GuiManager.Instance.SetCountdownVisibility(enable);
    }
    public static void Countdown(float countdown)
    {
        GuiManager.Instance.CountdownText(countdown);
    }

    public void CountdownText(float countdown)
    {
        CountdownWidgetText.text = string.Format("{0}", Mathf.Round(countdown));
    }

    public void ReplayButton()
    {
        AudioManager.PlaySound(null, "Click", 1);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ResumeButton()
    {
        GameEventSystem.Call(GameEventType.ToggleSlowMo, true);

        AudioManager.PlaySound(null, "Back", 1);
        ShowPauseMenu(false);
        GameManager.PauseTheGame(false);
    }

    public void PauseButton()
    {
        GameEventSystem.Call(GameEventType.ToggleSlowMo, false);

        AudioManager.PlaySound(null, "Click", 1);
        ShowPauseMenu(true);
        GameManager.PauseTheGame();
    }

    public void ShowPauseMenu(bool value)
    {
        PauseScreen.SetActive(value);
    }

    public void UpdateCoinWidgetText()
    {
        //TODO Update Coin Widget
        CoinWidgetText.text = string.Format("{0}", 0);
    }

    public void UpdateScore(int score)
    {
        //TODO Update Score
        string scoreText = string.Format("{00:00000000}", 0);
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
        GameManager.PauseTheGame(false);
        AudioManager.PlaySound(null, "Click", 1);
        SceneLoader.Instance.LoadMainenu();
        GameObject activeMenuGO = null;

        if (WinScreen.activeSelf)
        {
            activeMenuGO = WinScreen;
        }
        else if (GameOverScreen.activeSelf)
        {
            activeMenuGO = GameOverScreen;
        }
        else if (PauseScreen.activeSelf)
        {
            activeMenuGO = PauseScreen;
        }

        if (activeMenuGO != null)
            StartCoroutine(DelayCloseMenu(activeMenuGO, 1));
    }

    IEnumerator DelayCloseMenu(GameObject Menu, float time)
    {
        yield return new WaitForSeconds(time);
        Menu.SetActive(false);
    }

    public static void PlayTrasmition(string[] transmitions, bool boss = false)
    {
        AudioManager.PlaySound(null, "transmition", 3);
        if (GuiManager.Instance)
            GuiManager.Instance.ShowTrasmition(transmitions, boss);
    }

    public void ShowTrasmition(string[] transmitions, bool boss = false)
    {
        if (GameObject.FindObjectOfType<TransmitionWidget>())
            GameObject.FindObjectOfType<TransmitionWidget>().RecieveTransmition(transmitions, boss);
    }

    public static bool IsTrasnmiting()
    {
        if (GameObject.FindObjectOfType<TransmitionWidget>())
        {
            return GameObject.FindObjectOfType<TransmitionWidget>().IncomingTransmition;
        }
        else
        {
            return false;
        }
    }

    public void Win(GameController gc)
    {
        Time.timeScale = 1.0f;
        if (!ResultShowed)
        {
            PlayerShip player = PlayerManager.GetPlayer();
            PlayerData playerData = DataController.GetPlayerData();

            //TODO Exit Animatin

            playerData.Upgrades[((int)UpgradeType.Shield - 1)] = 0;

            if (player == null)
            {
                player = GameObject.FindObjectOfType<PlayerShip>();
            }

            playerData.Level = player.GetLevelSystem().GetLevel();
            playerData.xp = player.GetLevelSystem().GetXP();
            playerData.xpToLevel = player.GetLevelSystem().GetXpToLevel();

            //Save Game Data
            playerData.PlayedGame = true;
            for (int i = 0; i < playerData.ListOfOnGoingObjectives.Count; i++)
            {
                ObjectiveData objective = playerData.ListOfOnGoingObjectives[i];
                switch ((ObjectiveType)objective.objectiveType)
                {
                    case ObjectiveType.Kill:
                        
                        //TODO Current Enemy Killed
                        if (objective.completed == false)
                        {
                            var progressSoFar = objective.progress + 0;
                            objective.UpdateProgress(progressSoFar);
                        }

                        break;
                    case ObjectiveType.Use:
                        if (objective.completed == false)
                        {
                            var progressSoFar = objective.progress + player.GetWeaponSystem().SpecialAttack.superUsed;
                            objective.UpdateProgress(progressSoFar);
                        }
                        break;
                    case ObjectiveType.Unharmed:
                        if (objective.completed == false)
                        {
                            if (player.IsPlayerDamaged == false)
                            {
                                ObjectiveData objectiveData = playerData.GetOnGoingObjectiveById(ObjectiveType.Unharmed);
                                objectiveData.UpdateProgress(1);
                            }
                        }
                        break;
                    case ObjectiveType.survive:
                        //TODO WaveSurvived
                        objective.UpdateProgress(0);
                        break;
                    case ObjectiveType.spend:
                        break;
                    default:
                        break;
                }
            }


            Dictionary<string, LevelObjectiveData[]> Challanges = playerData.GetListOfObjectives();
            int missionsCompleted = 0;
            foreach (KeyValuePair<string, LevelObjectiveData[]> item in Challanges)
            {
                if (item.Value[0].completed == true)
                {
                    missionsCompleted++;
                }
            }

            int levelPlayed = GameManager.LevelSelected;
            //TODO Score
            playerData.SetScore(levelPlayed + 1,0);

            playerData.LevelUnlocked = missionsCompleted;


            if (GooglePlayServicesManager.Instance)
            {
                GooglePlayServicesManager.Instance.ReportAchivementProgress(EasyMobile.EM_GameServicesConstants.Achievement_Piece_of_Cake, playerData.TotalKills);
                GooglePlayServicesManager.Instance.ReportAchivementProgress(EasyMobile.EM_GameServicesConstants.Achievement_Destroyer, playerData.TotalKills);
            }

            StartCoroutine(WinCoroutine());
        }
    }
    //Game is Over
    public void GameOver(GameController gc)
    {
        Time.timeScale = 1.0f;
        if (!ResultShowed)
        {   
            PlayerData playerData = DataController.GetPlayerData();
            playerData.Upgrades[((int)UpgradeType.Shield - 1)] = 0;
            playerData.Level = playerShip.GetLevelSystem().GetLevel();
            playerData.xp = playerShip.GetLevelSystem().GetXP();
            playerData.xpToLevel = playerShip.GetLevelSystem().GetXpToLevel();

            //TODO Coins Earn In Game
            //TODO Enemy Killed In Game
            playerData.Coins += 0;
            playerData.TotalKills += 0;

            SaveSystem.SavePlayerData();


            if (GooglePlayServicesManager.Instance)
            {
                GooglePlayServicesManager.Instance.ReportAchivementProgress(EasyMobile.EM_GameServicesConstants.Achievement_Piece_of_Cake, playerData.TotalKills);
                GooglePlayServicesManager.Instance.ReportAchivementProgress(EasyMobile.EM_GameServicesConstants.Achievement_Destroyer, playerData.TotalKills);
            }

            ResultShowed = true;

            StartCoroutine(GameOverCoroutine());

        }
    }

    private IEnumerator GameOverCoroutine()
    {

        PlayerHUD.gameObject.SetActive(false);
        AudioManager.PlayMusic("GameOver", false);


        yield return new WaitForSeconds(2.0f);

        GameOverScreen.gameObject.SetActive(true);
    }

    public IEnumerator WinCoroutine()
    {
        PlayerHUD.gameObject.SetActive(false);
        yield return new WaitForSeconds(2.0f);
        AudioManager.PlayMusic("Victory", false);

        yield return new WaitForSeconds(2.0f);

        WinScreen.gameObject.SetActive(true);
    }
}