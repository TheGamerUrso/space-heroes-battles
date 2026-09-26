using TheGamerUrso.Core;
using TMPro;
using UnityEngine;

public class HighScoreUI : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    [SerializeField] private TextMeshProUGUI m_HighScore;
    [SerializeField] private TextMeshProUGUI m_PreviousScore;

    public GameObject Window;
    private IDataService dataService;
    private IAudioService audioService;


    private void Awake()
    {
        dataService = GameContext.Get<IDataService>();
        audioService = GameContext.Get<IAudioService>();
    }

    void Start () 
    {
        var playerData = dataService.GetPlayerData();
        string highscore = string.Format("{0:00000000}", playerData.HighScore);
        string score = string.Format("{0:00000000}", gameController.Score);

        m_HighScore.text = highscore;
        m_PreviousScore.text = score;
      
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
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
