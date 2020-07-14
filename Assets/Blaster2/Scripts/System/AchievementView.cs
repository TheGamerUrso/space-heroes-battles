using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementView : MonoBehaviour
{
    private Achievement achievement;
    private AchievementSystem achievementSystem;

   [SerializeField] private Image image;
    [SerializeField] private Image tick;

    [SerializeField] private TextMeshProUGUI Name;
    [SerializeField] private TextMeshProUGUI Description;

    public void Initialize(Achievement achievement,AchievementSystem achievementSystem)
    {
        this.achievement = achievement;
        this.achievementSystem = achievementSystem;
        UpdateAchievelemtView();
    }

    public void UpdateAchievelemtView()
    {
        if (achievement != null)
        {
            image.sprite = achievement.Icon;
            if (achievement.completed)
            {
                tick.gameObject.SetActive(true);
            }
            else
            {
                tick.gameObject.SetActive(false);
            }
            Name.text = achievement.Name;
            Description.text = achievement.Description;
        }
    }
}
