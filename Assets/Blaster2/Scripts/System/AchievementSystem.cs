using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementSystem
{
    public static AchievementSystem instance;
    public static AchievementSystem Instance
    {
        get
        {
            return instance;
        }
    }

    public List<Achievement> ListOfAchievement = new List<Achievement>();
   
    public AchievementSystem(List<Achievement> ListOfAchievement)
    {
        if(instance == null)
        {
            instance = this;
        }
        this.ListOfAchievement = ListOfAchievement;
    }

    public void Progress(int index, int value)
    {
        for (int i = 0; i < ListOfAchievement.Count; i++)
        {
            if (ListOfAchievement[index].ID == index)
            {
                if (ListOfAchievement[index].Check())
                {
                    ListOfAchievement[index].progress += value;
                }
                Check(index);
                return;
            }
        }
    }

    public void Check(int index)
    {
        for (int i = 0; i < ListOfAchievement.Count; i++)
        {
            if (ListOfAchievement[i].ID == index)
            {
                if (ListOfAchievement[i].Check())
                {
                    Complete(ListOfAchievement[i]);
                }
            }
            return;
        }
    }

    public void Complete(Achievement achievement)
    {
        if (!achievement.completed)
        {
            achievement.completed = true;
            achievement.Complete();
            Notification notification = new Notification();
            notification.Name = achievement.Name;
            notification.icon = PersistantData.Instance.GetAchievementIcon(achievement.ID);
            notification.Description = achievement.Description;
            NotificationSystem.Instance.Add(notification);
        }
    }
}
