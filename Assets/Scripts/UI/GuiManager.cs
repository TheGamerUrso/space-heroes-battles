using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [Header("PlayerHUD")]
    [SerializeField] private GameObject PlayerHUD;
    [SerializeField] private GameObject SpecialIsReadyFeedback;
    [SerializeField] private TextMeshProUGUI ScoreText;
    [SerializeField] private TextMeshProUGUI CoinWidgetText;
    [SerializeField] private TextMeshProUGUI CountdownWidgetText;


    private bool ResultShowed = false;
    #endregion Variables

    private void OnApplicationFocus(bool focus)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (GameManager.instance.GetCurrentState() != GameStates.Game)
            {
                return;
            }

            if (!focus && SpawnEnemies.GameOver == false)
            {
                GameManager.instance.PauseTheGame();
                ShowPauseMenu(true);
            }
        }
    }

    private void OnApplicationPause(bool Paused)
    {
        if (GameManager.instance.GetCurrentState() != GameStates.Game)
        {
            return;
        }

        if (Application.platform == RuntimePlatform.Android)
        {
            if (SpawnEnemies.GameOver == false)
            {
                GameManager.instance.PauseTheGame();
                ShowPauseMenu(true);
            }
        }
    }

    private void Start()
    {
        instance = this;
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
        GameManager.instance.SetState(GameStates.Game);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ResumeButton()
    {
        AudioManager.PlaySound("Back", 1);
        ShowPauseMenu(false);
        GameManager.instance.PauseTheGame(false);
    }

    public void PauseButton()
    {
        AudioManager.PlaySound("Click", 1);
        ShowPauseMenu(true);
        GameManager.instance.PauseTheGame();
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
        AudioManager.PlaySound("Click", 1);
        SceneLoader.instance.LoadMainenu();
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

    public static void PlayTrasmition(string[] transmitions)
    {
        AudioManager.PlaySound("transmition", 3);
        GuiManager.instance.ShowTrasmition(transmitions);
    }

    public void ShowTrasmition(string[] transmitions)
    {
        if (GameObject.FindObjectOfType<TransmitionWidget>())
            GameObject.FindObjectOfType<TransmitionWidget>().RecieveTransmition(transmitions);
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
        AudioManager.SetMusic("GameOver", false);


        yield return new WaitForSeconds(2.0f);



        GameManager.instance.SetState(GameStates.GameOver);



        GameOverScreen.gameObject.SetActive(true);
    }

    public IEnumerator WinCoroutine()
    {

        yield return new WaitForSeconds(2.0f);
        AudioManager.SetMusic("Victory", false);
        GameManager.instance.SetState(GameStates.GameOver);

        yield return new WaitForSeconds(2.0f);

        WinScreen.gameObject.SetActive(true);
    }
}