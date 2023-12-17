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
           //Achievement_Pieceofcake
                break;
            case 11:
                //Achievement_Destroyer
                break;
            case 12:
          //Achievement_HeroesAssemble
                break;
            case 13:
                //Achievement_MaxPower
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
         //Achievement_Prologue
                    break;
                case 1:
           //Achievement_Level2
                    break;
                case 2:
              //Achievement_Level3
                    break;
                case 3:
                 //Achievement_Level4
                    break;
                case 4:
               //Achievement_Level5
                    break;
                case 5:
                  //Achievement_Level6
                    break;
                case 7:
                //Achievement_Level7
                    break;
                case 8:
                //Achievement_Level8
                    break;
                case 9:
                   //Achievement_Level9
                    break;
                case 10:
             //Achievement_Pieceofcake
                    break;
                case 11:
                   //Achievement_Destroyer
                    break;
                case 12:
                    //Achievement_HeroesAssemble
                    break;
                case 13:
                    //Achievement_MaxPower
                    break;
                default:
                    break;
            }
  

            Notification notification = new Notification();
            notification.Name = Name;
            notification.icon = PersistantData.Instance.GetAchievementIcon(ID);
            notification.Description = Description;
            NotificationSystem.Instance.Add(notification);
        }
    }
}
