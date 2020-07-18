using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create New Upgrade")]
public class UpgradeData : ScriptableObject {
    public UpgradeTypeEnum upgradeType;
    public Sprite sprite;
    public int Cost;
    public int MaxLevel;
    public int[] CostPerLevel;
    public int[] LevelRequirementPerLevel;
}
