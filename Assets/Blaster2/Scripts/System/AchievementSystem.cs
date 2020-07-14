using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementSystem : MonoBehaviour
{
    List<Achievement> ListOfAchievement = new List<Achievement>();

    public AchievementList achievementList;
    public GameObject achievelemtViewElement;
    public Transform container;
    
    private void Start()
    {
        for (int i = 0; i < achievementList.ListOfAchievelemtnts.Count; i++)
        {
            GameObject element = Instantiate(achievelemtViewElement, container, false);
            element.GetComponent<AchievementView>().Initialize(achievementList.ListOfAchievelemtnts[i], this);
        }
    }

    public void Check(int index, int value)
    {
        for (int i = 0; i < ListOfAchievement.Count; i++)
        {
            if (ListOfAchievement[i].ID == index)
            {
                if (ListOfAchievement[i].Check(value))
                {
                    Complete(ListOfAchievement[i]);
                }
            }
        }
    }

    public void Complete(Achievement achievement)
    {
        if (!achievement.completed)
        {
            achievement.completed = true;
            achievement.Complete();
        }
    }
}
