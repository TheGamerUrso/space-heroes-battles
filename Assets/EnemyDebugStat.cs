using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyDebugStat : MonoBehaviour
{
    public BaseEnemy enemy;
    public TextMeshProUGUI statText;

    void Start()
    {
        statText.text = "iLv:" + enemy.Level + "\n" + "HP:" + enemy.MaxHealth + "\n" + "Dmg:" + enemy.Damage + "\n" + "FR:" + enemy.FireRate; 

    }

}
