using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class HighScoreWidget : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_HighScore;
    [SerializeField] private TextMeshProUGUI m_PreviousScore;

   // private AudioManager audioManager;
    public GameObject Window;

    void Start () {
        PlayerData playerData = PersistantData.GetPlayerData();
        string highscore = string.Format("{0:0000000000}", playerData.HighScore);
        string score = string.Format("{0:0000000000}", GameController.Instance.Score);

        m_HighScore.text = highscore;
        m_PreviousScore.text = score;
       // audioManager = AudioManager.instance;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //if (audioManager)
            //  audioManager.PlaySoundSFX(Camera.main,"close");
            CloseWindow();
        }

    }

    public void UpdateHighScore(float highscore)
    {
        string highscoreText = string.Format("High Score:\n{0}", highscore);
        m_HighScore.text = highscoreText;
    }

    public void UpdatePreviousGameScore(float score)
    {
        string scoreText = string.Format("Score:\n{0}", score);
        m_PreviousScore.text = scoreText;
    }

    public void CloseWindow()
    {
        Window.SetActive(false);
    }

    public void OpenWindow()
    {
        Window.SetActive(true);
    }

}
