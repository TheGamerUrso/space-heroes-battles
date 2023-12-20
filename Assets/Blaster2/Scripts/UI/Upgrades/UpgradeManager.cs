using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoSingleton<UpgradeManager>
{
    public List<UpgradeData> ListOfUpgrade = new List<UpgradeData>();
    public List<Upgrade> UpgradeDatabase = new List<Upgrade>();

    public void Start()
    {
        for (int i = 0; i < ListOfUpgrade.Count; i++)
        {
            Upgrade upgrade = new Upgrade(ListOfUpgrade[i]);
            UpgradeDatabase.Add(upgrade);
        }
    }

    public List<Upgrade> GetUpgradeDatabase()
    {
        return UpgradeDatabase;
    }

}