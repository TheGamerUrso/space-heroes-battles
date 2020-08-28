using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public enum ObjectiveTypeEnum
{
    KILL = 0, USE = 1, UNHARMED = 2, SURVIVE = 3, SPEND = 4, BOUNTY = 5, SCORE = 6
}

[Serializable]
public class ObjectiveData
{
    public bool claimed;
    public bool completed;
    public int pos;
    public string Id;
    public int progress;
    public int requirment;
    public int objectiveType;
    public string Description;

    public ObjectiveData(int pos,
        string Id,
        int requirment,
        int progress,
        int objectiveType,
        string description)
    {
        this.pos = pos;
        this.Id = Id;
        this.requirment = requirment;
        this.progress = progress;
        this.objectiveType = objectiveType;
        string newText = "" + requirment;

        string newString = description.Replace("%", newText);

        this.Description = newString;
    }
    public bool IsCompleted()
    {
        if (progress >= requirment && !completed)
        {
            return true;
        }
        return false;
    }

    public void UpdateProgress(int progress)
    {
        if (!completed)
        {
            this.progress += progress;

            if (this.progress >= requirment)
            {
                completed = true;
                progress = 0;

                Notification notification = new Notification();
                notification.Name = Id;
                notification.Description = Description;

                NotificationSystem.Instance.Add(notification);


                AnalyticsResult analyticsResults = Analytics.CustomEvent(
                   Id + " Challenge Completed");

                Debug.Log("analyticsResults:" + analyticsResults);
            }
        }

        progress = Mathf.Clamp(progress, 0, requirment);
    }
}
