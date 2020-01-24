using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using EasyMobile;
public class GuiManager : MonoBehaviour
{

    private static GuiManager instance;

    public static GuiManager Instance
    {
        get
        {
            return instance;
        }
        private set { instance = value; }
    }

    #region Variables

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

    private float slowMo;
    private bool useSloMo;
    private bool ResultShowed = false;
    #endregion Variables
    private float timer;

    private float delayTheSlowMoEffectTimer;

    public void ToggleSlowMo(bool value)
    {
        useSloMo = value;
        if (value == false)
        {
            Time.timeScale = 1.0f;
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (!focus && GameManager.IsGameOver == false)
            {
                GameManager.PauseTheGame();
                ShowPauseMenu(true);
            }
        }
    }

    private void OnApplicationPause(bool Paused)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (GameManager.IsGameOver == false)
            {
                GameManager.PauseTheGame();
                ShowPauseMenu(true);
            }
        }
    }

    private void Start()
    {
        instance = this;
        delayTheSlowMoEffectTimer = 4;
        timer = 1;
    }
    private void Update()
    {
        if (Time.timeScale == 1)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                //pauseButton.SetActive(false);
            }
        }
        else
        {
            timer = 1;
            pauseButton.SetActive(true);
        }
        SlowMoEffect();

        if (SpawnEnemies.Instance.spawnReady)
        {
            useSloMo = true;
        }

    }

    public void SlowMoEffect()
    {
        if (useSloMo && !GameManager.Paused)
         {
            if (IsTrasnmiting() || Input.touchCount > 0 || Input.GetMouseButton(0))
            {
                slowMo = 1;
            }
            else
            {
                slowMo = .3f;

            }
            Time.timeScale = slowMo;
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
        GuiManager.instance.CountdownText(countdown);
    }

    public void CountdownText(float countdown)
    {
        CountdownWidgetText.text = string.Format("{0}", Mathf.Round(countdown));
    }

    public void ReplayButton()
    {
        AudioManager.PlaySound("Click", 1);
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ResumeButton()
    {
        useSloMo = true;
        AudioManager.PlaySound("Back", 1);
        ShowPauseMenu(false);
        GameManager.PauseTheGame(false);
    }

    public void PauseButton()
    {
        useSloMo = false;
        AudioManager.PlaySound("Click", 1);
        ShowPauseMenu(true);
        GameManager.PauseTheGame();
    }

    public void ShowPauseMenu(bool value)
    {
        PauseScreen.SetActive(value);
    }

    public void UpdateCoinWidgetText()
    {
        CoinWidgetText.text = string.Format("{0}", SpawnEnemies.counsEarnInGame);
    }

    public void UpdateScore(int score)
    {
        string scoreText = string.Format("{00:00000000}", SpawnEnemies.Score);
        ScoreText.text = scoreText;
    }

    public void CreateFloatingText(string text, Vector3 pos)
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
        useSloMo = false;
        GameManager.PauseTheGame(false);
        AudioManager.PlaySound("Click", 1);
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
        AudioManager.PlaySound("transmition", 3);
        GuiManager.instance.ShowTrasmition(transmitions, boss);
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

    //Game is Over
    public void GameOver()
    {
        Time.timeScale = 1.0f;
        if (!ResultShowed)
        {
            Player player = PlayerManager.GetPlayer();

            if (player == null)
            {
                player = GameObject.FindObjectOfType<Player>();
            }
            PlayerData playerData = DataController.GetPlayerData();

            playerData.Level = player.GetLevelSystem().GetLevel();
            playerData.xp = player.GetLevelSystem().GetXP();
            playerData.xpToLevel = player.GetLevelSystem().GetXpToLevel();


            if (player.GetHealthPresentage() > 0)
            {
                //Save Game Data
                playerData.PlayedGame = true;
                for (int i = 0; i < playerData.ListOfOnGoingObjectives.Count; i++)
                {
                    ObjectiveData objective = playerData.ListOfOnGoingObjectives[i];
                    switch ((ObjectiveType)objective.objectiveType)
                    {
                        case ObjectiveType.Kill:
                            if (objective.completed == false)
                            {
                                var progressSoFar = objective.progress + SpawnEnemies.CurrentEnemyKilled;
                                objective.UpdateProgress(progressSoFar);
                            }
                            break;
                        case ObjectiveType.Use:
                            if (objective.completed == false)
                            {
                                var progressSoFar = objective.progress + player.GetWeaponSystem().GetHowManyTimesSuperIsUsed();
                                objective.UpdateProgress(progressSoFar);
                            }
                            break;
                        case ObjectiveType.Unharmed:
                            if (objective.completed == false)
                            {
                                if (player.IsPlayerDamaged() == false)
                                {
                                    ObjectiveData objectiveData = playerData.GetOnGoingObjectiveById(ObjectiveType.Unharmed);
                                    objectiveData.UpdateProgress(1);
                                }
                            }
                            break;
                        case ObjectiveType.survive:
                            objective.UpdateProgress(SpawnEnemies.WaveSurvived);
                            break;
                        case ObjectiveType.spend:
                            break;
                        default:
                            break;
                    }
                }
            }

            //playerData.GotHitInGame = player.IsPlayerDamaged();
            // playerData.TotalSuperUsed = player.GetWeaponSystem().GetHowManyTimesSuperIsUsed();
            // playerData.WaveSurvived += SpawnEnemies.WaveSurvived;
            // playerData.m_EnemyKilled += SpawnEnemies.EnemyKilled;
            //playerData.TotalSuperUsed += player.GetWeaponSystem().GetHowManyTimesSuperIsUsed();



            if (!SpawnEnemies.Instance.survival)
            {
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

                playerData.SetScore(levelPlayed + 1, SpawnEnemies.Score);

                playerData.LevelUnlocked = missionsCompleted;
            }
            else
            {
                playerData.SetScore(0, SpawnEnemies.Score);
            }

            playerData.Coins += SpawnEnemies.counsEarnInGame;
            playerData.TotalKills += SpawnEnemies.EnemyKilled;

            SaveSystem.SavePlayerData();

            if (GooglePlayServicesManager.Instance)
            {
               GooglePlayServicesManager.Instance.ReportAchivementProgress(EasyMobile.EM_GameServicesConstants.Achievement_Piece_of_Cake, playerData.TotalKills);
               GooglePlayServicesManager.Instance.ReportAchivementProgress(EasyMobile.EM_GameServicesConstants.Achievement_Destroyer, playerData.TotalKills);
            }


            ResultShowed = true;

            if (player.GetHealthPresentage() > 0)
            {
                StartCoroutine(WinCoroutine());
            }
            else
            {
                StartCoroutine(GameOverCoroutine());
            }
        }
    }

    private IEnumerator GameOverCoroutine()
    {
        useSloMo = false;
        PlayerHUD.gameObject.SetActive(false);
        AudioManager.SetMusic("GameOver", false);


        yield return new WaitForSeconds(2.0f);

        GameOverScreen.gameObject.SetActive(true);
    }

    public IEnumerator WinCoroutine()
    {
        useSloMo = false;
        PlayerHUD.gameObject.SetActive(false); 
        yield return new WaitForSeconds(2.0f);
        AudioManager.SetMusic("Victory", false);

        yield return new WaitForSeconds(2.0f);

        WinScreen.gameObject.SetActive(true);
    }
}