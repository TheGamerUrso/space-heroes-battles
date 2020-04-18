using System.Collections;
using System.Collections.Generic;
using EasyMobile;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameResults : MonoBehaviour
{
    protected PlayerShipElement player;
    protected PlayerData playerData;

    [SerializeField] private MissionCollection missionCollection;
    [SerializeField] private Mission mission;
    protected string levelName;
    protected float killed;
    protected float collected;

    public TextMeshProUGUI m_Text;
    public Button m_PlayAgainButton;
    public Button m_QuitButton;
    public GameObject m_Canvas;
    protected Animator animator;

    [Header("GameOver Widget Config")]
    public LevelObjectivesElement[] levelObjectives;
    public LevelObjectiveData[] levelObjectiveDatas;


    public AudioClip audioClip;
    public AudioSource audioSource;

    public GameObject highscore;

    public void PlaySound()
    {
        audioSource.PlayOneShot(audioClip);
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ShowResults()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        PlayerShip player = PlayerManager.GetPlayer();
        PlayerData playerData = GameManager.Instance.GetPlayerData();

        string scoreText = string.Format("{00:0000000000}", GameSession.score);
        m_Text.text = scoreText;

        levelName = "Level" + GameManager.LevelSelected;
        highscore.SetActive(false);
        playerData.SurvivalScore = GameSession.score;
        if (playerData.SurvivalScore > playerData.SurvivalHighScore)
        {
            highscore.SetActive(true);
            playerData.SurvivalHighScore = playerData.SurvivalScore;
        }

        // Unlock an achievement
        // EM_GameServicesConstants.Sample_Achievement is the generated name constant
        // of an achievement named "Sample Achievement"
#if UNITY_ANDROID
        if (GooglePlayServicesManager.Instance)
            GooglePlayServicesManager.Instance.ReportLeaderboards(playerData.SurvivalHighScore, EM_GameServicesConstants.Leaderboard_Survival_Mode);
#elif UNITY_EDITOR
     Debug.Log("UnlockAchievement"); 
#endif
    }

}
