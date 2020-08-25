using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

[Serializable]
public class Achievement
{
    public bool completed;
    public int ID;
    public string Name;
    public string Description;
    public int progress;
    public int requirement;
    public string achievementID;

    public void Report(int value)
    {
        progress = value;
    }

    public void Check()
    {
        if (!completed && progress >= requirement)
        {
            completed = true;

            GPServices.ReportAchievementProgress(achievementID, progress);

            Notification notification = new Notification();
            notification.Name = Name;
            notification.icon = PersistantData.Instance.GetAchievementIcon(ID);
            notification.Description = Description;
            NotificationSystem.Instance.Add(notification);

            AnalyticsResult analyticsResults = Analytics.CustomEvent(" Achievement Unlocked" + Name);
            Debug.Log("analyticsResults:" + analyticsResults);
        }
    }
}
