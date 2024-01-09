using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public enum QuestTypeEnum
{
    KILL = 0, USE = 1, UNHARMED = 2, SURVIVE = 3, SPEND = 4, BOUNTY = 5, SCORE = 6
}

[Serializable]
public class QuestData
{
    public bool claimed;
    public bool completed;
    public int pos;
    public string Id;
    public int progress;
    public int requirment;
    public int questType;
    public string Description;

    public QuestData(int pos,
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
        this.questType = objectiveType;
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
            this.progress = Mathf.Clamp(this.progress, 0, requirment);
            if (this.progress >= requirment)
            {
                completed = true;
                Notification notification = new Notification();
                notification.Name = Id;
                this.Description = Description.Replace("%",""+ requirment);    
                notification.Description = Description.Replace(" X ", "" + progress);
                NotificationSystem.Instance.Add(notification);
                progress = 0;         
            }
        }
    
    }
}
