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

        PlayerData playerData = PersistantData.GetPlayerData();

        string scoreText = string.Format("{00:0000000000}", Game.Score);
        m_Text.text = scoreText;

        highscore.SetActive(false);

        var score = playerData.GetScore(GameManager.LevelIndexSelected);
        var hscore = playerData.GetHighScore(GameManager.LevelIndexSelected);

        if (Game.IsHightScore)
        {
            highscore.SetActive(true);
        }

        Game.IsHightScore = false;

        Game.IsSurvivalMode = false;

    }

}
