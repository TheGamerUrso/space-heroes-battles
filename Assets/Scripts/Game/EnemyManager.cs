using System.Collections.Generic;
using TheGamerUrso.PoolSystem;
using UnityEngine;

public class EnemyManager
{
    private static EnemyManager instance;

    public static EnemyManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new EnemyManager();
            }
            return instance;
        }
    }
   
}