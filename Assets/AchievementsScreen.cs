using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementsScreen : MonoBehaviour
{
    List<Achievement> ListOfAchievement = new List<Achievement>();
    public GameObject achievelemtViewElement;
    public Transform container;


    private void Start()
    {
        ListOfAchievement = AchievementSystem.instance.ListOfAchievement;

        for (int i = 0; i < ListOfAchievement.Count; i++)
        {
            GameObject element = Instantiate(achievelemtViewElement, container, false);
            element.GetComponent<AchievementView>().Initialize(ListOfAchievement[i], this);
        }
    }
 
}
