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
        if (instance == null)
        {
            instance = this;
        }
        this.ListOfAchievement = ListOfAchievement;
    }

    public void Report(int Id, int value)
    {
        foreach (var item in ListOfAchievement)
        {
            if (item.ID == Id)
            {
                item.Report(value);

                item.Check();
                return;
            }
        }
    }
}
