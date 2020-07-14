using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Achievement List",menuName = "Blaster2/New Achievement List")]
public class AchievementList : ScriptableObject
{
    public List<Achievement> ListOfAchievelemtnts = new List<Achievement>();

    public void Reorder()
    {
        for (int i = 0; i < ListOfAchievelemtnts.Count; i++)
        {
            ListOfAchievelemtnts[i].ID = i;
        }
    }
}
