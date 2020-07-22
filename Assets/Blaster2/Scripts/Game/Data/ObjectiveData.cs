using System;
using UnityEngine;

public enum ObjectiveType
{
    Kill = 0, Use = 1, Unharmed = 2, survive = 3, spend = 4
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
            this.progress = progress;

            if (progress >= requirment)
            {
                completed = true;
                progress = 0;

                Notification notification = new Notification();
                notification.Description = Description;

                NotificationSystem.Instance.Add(notification);

                //if (ObjectiveCompleteNotification.Instance != null)
                //    ObjectiveCompleteNotification.Instance.AddToQue(this);
            }
        }

        progress = Mathf.Clamp(progress, 0, requirment);
    }
}
