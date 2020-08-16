using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementView : MonoBehaviour
{
    private Achievement achievement;
    private AchievementsScreen achievementsScreen;


    [SerializeField] private Image image;
    [SerializeField] private Image tick;

    [SerializeField] private TextMeshProUGUI Name;
    [SerializeField] private TextMeshProUGUI Description;

    public void Initialize(Achievement achievement, AchievementsScreen achievementsScreen)
    {
        this.achievement = achievement;
        this.achievementsScreen = achievementsScreen;
        UpdateAchievelemtView();
    }

    public void UpdateAchievelemtView()
    {
        if (achievement != null)
        {
            image.sprite = PersistantData.Instance.GetAchievementIcon(achievement.ID);
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
