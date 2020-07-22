using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameScoreWidget : MonoBehaviour
{
    public TextMeshProUGUI ScoreText;
    private int score = 0;
    private float speed = 0.5f;
    private float scoreToShow = 0;

    private void OnDestroy()
    {
        Events.OnScoreValueChanged -= (x) =>
        {
            score = Game.GetScore();
        };
    }

    void Start()
    {
        Events.OnScoreValueChanged += (x) =>
        {
            score = Game.GetScore();
        };
    }

    private void Update()
    {     
        scoreToShow = Mathf.Lerp(scoreToShow, (float)score, speed);
        SetScore(scoreToShow);
    }

    public void SetScore(float score)
    {
        ScoreText.text = string.Format("{0:0000000000}", score); ;
    }
}
