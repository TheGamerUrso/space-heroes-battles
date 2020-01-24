using EasyMobile;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverWidget : MonoBehaviour
{
    private Player player;
    private PlayerData playerData;

    [SerializeField] private MissionCollection missionCollection;
    [SerializeField] private Mission mission;
    private string levelName;
    private float killed;
    private float collected;

    public TextMeshProUGUI m_Text;
    public Button m_PlayAgainButton;
    public Button m_QuitButton;
    public GameObject m_Canvas;
    private Animator animator;

    [Header("GameOver Widget Config")]
    public LevelObjectivesElement[] levelObjectives;
    public LevelObjectiveData[] levelObjectiveDatas;
    public void PlaySound()
    {
        AudioManager.PlaySound("UISlide", 0);
    }
    private void OnEnable()
    {
        GameManager.IsGameOver = true;

        Player player = PlayerManager.GetPlayer();
        PlayerData playerData = DataController.GetPlayerData();
        PlayerAnimation playerAnimation = player.PlayerAnimation();

        if (player != null)
        {
            AudioManager.PlaySound("Victory", 3);
            playerAnimation.Exit();
        }
        else if (player == null)
        {
            AudioManager.PlaySound("GameOver", 3);
        }

        UpdateScore();
        AudioManager.SetMusic("GameOver");
        levelName = "Level" + GameManager.LevelSelected;
        killed = EnemyManager.EnemySpawnedInTotal * .9f;
        collected = SpawnEnemies.CoinDropInTotal * .9f;

        playerData.Upgrades[((int)UpgradeType.Shield - 1)] = 0;

        if (SpawnEnemies.Instance.survival == false)
        {
            missionCollection = DataController.GetMissionCollection();
            mission = missionCollection.GetMission(GameManager.LevelSelected);
    
            levelObjectiveDatas = playerData.GetLevelObjectives(levelName);
        }
        if (!SpawnEnemies.Instance.survival)
        {
            StartCoroutine(ShowGameResults());
        }
    }


    private IEnumerator ShowGameResults()
    {
        int ChallengeIndex = 0;

        foreach (LevelObjectivesElement item in levelObjectives)
        {
            item.gameObject.SetActive(false);
        }

        for (int i = 0; i < levelObjectives.Length; i++)
        {
            levelObjectives[i].levelObjectiveData = levelObjectiveDatas[i];
        }

        yield return new WaitForSeconds(1.0f);

        levelObjectives[ChallengeIndex].gameObject.SetActive(true);

        levelObjectives[ChallengeIndex].RefreshLevelObjectiveEement();

        levelObjectives[ChallengeIndex].CheckComplete();

        ChallengeIndex = 1;

        yield return new WaitForSeconds(1.0f);

        levelObjectives[ChallengeIndex].gameObject.SetActive(true);

        levelObjectives[ChallengeIndex].RefreshLevelObjectiveEement();

        levelObjectives[ChallengeIndex].CheckComplete();

        ChallengeIndex = 2;

        yield return new WaitForSeconds(1.0f);

        levelObjectives[ChallengeIndex].gameObject.SetActive(true);

        levelObjectives[ChallengeIndex].RefreshLevelObjectiveEement();

        levelObjectives[ChallengeIndex].CheckComplete();

        ChallengeIndex = 3;

        yield return new WaitForSeconds(1.0f);

        levelObjectives[ChallengeIndex].gameObject.SetActive(true);

        levelObjectives[ChallengeIndex].RefreshLevelObjectiveEement();

        levelObjectives[ChallengeIndex].CheckComplete();

        int num = 0;

        for (int i = 0; i < levelObjectives.Length; i++)
        {
            if (levelObjectives[i].levelObjectiveData.completed)
            {
                num++;
            }
        }

        if(num == 4)
        {
            // Unlock an achievement
            // EM_GameServicesConstants.Sample_Achievement is the generated name constant
            // of an achievement named "Sample Achievement"
#if UNITY_ANDROID
            if (GooglePlayServicesManager.Instance)
            GooglePlayServicesManager.Instance.UnlockAchievement(GameManager.LevelSelected);
#elif UNITY_EDITOR
     Debug.Log("UnlockAchievement"); 
#endif
        }
    }
    public void UpdateScore()
    {
        string scoreText = string.Format("{00:0000000000}", SpawnEnemies.Score);
        UpdateText(scoreText);
    }

    private void Start()
    {
        animator = GetComponent<Animator>();

        m_PlayAgainButton.onClick.AddListener(() =>
        {
            Scene loadedLevel = SceneManager.GetActiveScene();
            SceneLoader.Instance.LoadScene(loadedLevel.name);
        });

        m_QuitButton.onClick.AddListener(() =>
        {
            SceneLoader.Instance.LoadScene("Main");
        });
    }

    public void UpdateText(string text)
    {
        m_Text.text = text;
    }
}