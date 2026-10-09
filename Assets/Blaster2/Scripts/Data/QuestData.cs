using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public enum QuestTypeEnum
{
    KILL = 0, USE = 1, UNHARMED = 2, SURVIVE = 3, SPEND = 4, BOUNTY = 5, SCORE = 6
}

[Serializable]
public class QuestReward
{
    public float XP;
    public int Currency;
    public QuestReward(float XP,int Currency)
    {
        this.XP = XP;
        this.Currency = Currency;
    }
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
    public QuestReward questReward;

    public QuestData(int pos,
        int requirment,
        QuestTypeEnum questType,
        string description,
        QuestReward questReward)
    {
        this.pos = pos;
        this.questType = (int)questType;
        this.Id = GetQuestNameByType(questType);
        this.requirment = requirment;
        this.progress = 0;
     
        string newText = "" + requirment;

        string newString = description.Replace("%", newText);

        this.Description = newString;
        this.questReward = questReward;
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
                this.Description = Description.Replace("%",""+ requirment);    
                progress = 0;         
            }
        }
    }

    public string GetQuestNameByType(QuestTypeEnum questType)
    {
        switch (questType)
        {
            case QuestTypeEnum.KILL:
                return "DEFEAT ENEMIES";
            case QuestTypeEnum.USE:
                return "SPECIAL ATTACK USED";
            case QuestTypeEnum.UNHARMED:
                return "UNHARMED";
            case QuestTypeEnum.SURVIVE:
                return "SURVIVE";
            case QuestTypeEnum.SPEND:
                return "CAPITALISM";
            case QuestTypeEnum.BOUNTY:
                return "BOUNTY";
            case QuestTypeEnum.SCORE:
                return "SCORE";
        }
        return "";
    }

    public string GetQuestDescriptionByType(QuestTypeEnum questType)
    {
        if (questType == QuestTypeEnum.UNHARMED)
        {
            return Description;
        }
        else if (questType == QuestTypeEnum.BOUNTY ||
            questType == QuestTypeEnum.SURVIVE)
        {
           return Description.Replace(
                " X ", "" + requirment);
        }
        else
        {
            return Description.Replace(" X ", "" + progress);
        }
    }
}
