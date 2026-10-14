using TMPro;
using UnityEngine;
using PrimeTween;

public class GameScoreWidget : MonoBehaviour
{
    public GameController gameController;
    public TextMeshProUGUI ScoreText;
    private int score = 0;
    private float speed = 0.5f;
    private float scoreToShow = 0;

    private void OnScoreValueChanged(int Score){
         score = (int)Score;
    }
    private void Start()
    {
        gameController.OnGameScoreValueChanged += OnScoreValueChanged;
        SetScore(0);
    }

    private void OnDestroy()
    {
        gameController.OnGameScoreValueChanged -= OnScoreValueChanged;
        
    }

    private void Update()
    {     
        scoreToShow = Mathf.Lerp(scoreToShow, (float)score, speed);
        SetScore(scoreToShow);
    }

    public void SetScore(float score)
    {
        string scoreText = string.Format("{00:00000000}", score);
        ScoreText.text = scoreText;
        Tween.Scale(transform, endValue: 1.2f, duration: 1, ease: Ease.InOutSine);


    }
}
