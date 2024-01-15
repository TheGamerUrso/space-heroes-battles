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
        Events.OnScoreValueChanged -= OnScoreValueChanged;
    }

    void Start()
    {
        Events.OnScoreValueChanged += OnScoreValueChanged;
    }
    private void OnScoreValueChanged(int Score){
         score = (int)Score;
    }
    private void Update()
    {     
        scoreToShow = Mathf.Lerp(scoreToShow, (float)score, speed);
        SetScore(scoreToShow);
    }

    public void SetScore(float score)
    {
        ScoreText.text = string.Format("{0:00000000}", score); ;
    }
}
