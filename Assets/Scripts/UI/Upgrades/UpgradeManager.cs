using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public UpgradeElement[] upgradeElements;
    private void Start()
    {
        for (int i = 0; i < upgradeElements.Length; i++)
        {
            upgradeElements[i].RefreshUpgradeElement();
            upgradeElements[i].OnUpgradeBought = Refresh;
        }
    }

    public void RefreshUpgrades()
    {
        for (int i = 0; i < upgradeElements.Length; i++)
        {
            upgradeElements[i].RefreshUpgradeElement();
        }
    }

    public void Refresh(UpgradeElement upgradeElement)
    {
        PlayerData playerData = DataController.GetPlayerData();
        playerData.SetUpgrade(upgradeElement);
        RefreshUpgrades();
    }

    public void Save()
    {
        SaveSystem.SavePlayerData();
    }

}