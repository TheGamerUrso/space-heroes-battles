using EasyMobile;
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
        switch (ID)
        {        
            case 10:
                GPServices.ReportAchievementProgress(EM_GameServicesConstants.Achievement_Pieceofcake, progress);
                break;
            case 11:
                GPServices.ReportAchievementProgress(EM_GameServicesConstants.Achievement_Destroyer, progress);
                break;
            case 12:
                GPServices.ReportAchievementProgress(EM_GameServicesConstants.Achievement_HeroesAssemble, progress);
                break;
            case 13:
                GPServices.ReportAchievementProgress(EM_GameServicesConstants.Achievement_MaxPower, progress);
                break;
        }
    }

    public void Check()
    {
        if (!completed && progress >= requirement)
        {
            completed = true;
            switch (ID)
            {
                case 0:
                    GPServices.UnlockAchievement(EM_GameServicesConstants.Achievement_Prologue);
                    break;
                case 1:
                    GPServices.UnlockAchievement(EM_GameServicesConstants.Achievement_Level2);
                    break;
                case 2:
                    GPServices.UnlockAchievement(EM_GameServicesConstants.Achievement_Level3);
                    break;
                case 3:
                    GPServices.UnlockAchievement(EM_GameServicesConstants.Achievement_Level4);
                    break;
                case 4:
                    GPServices.UnlockAchievement(EM_GameServicesConstants.Achievement_Level5);
                    break;
                case 5:
                    GPServices.UnlockAchievement(EM_GameServicesConstants.Achievement_Level6);
                    break;
                case 7:
                    GPServices.UnlockAchievement(EM_GameServicesConstants.Achievement_Level7);
                    break;
                case 8:
                    GPServices.UnlockAchievement(EM_GameServicesConstants.Achievement_Level8);
                    break;
                case 9:
                    GPServices.UnlockAchievement(EM_GameServicesConstants.Achievement_Level9);
                    break;
                case 10:
                    GPServices.UnlockAchievement(EM_GameServicesConstants.Achievement_Pieceofcake);
                    break;
                case 11:
                    GPServices.UnlockAchievement(EM_GameServicesConstants.Achievement_Destroyer);
                    break;
                case 12:
                    GPServices.UnlockAchievement(EM_GameServicesConstants.Achievement_HeroesAssemble);
                    break;
                case 13:
                    GPServices.UnlockAchievement(EM_GameServicesConstants.Achievement_MaxPower);
                    break;
                default:
                    break;
            }
  

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
