using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public UpgradeElement[] upgradeElements;
    private void Start()
    {
        GameEventSystem.OnUpgradeBought = Refresh;

        for (int i = 0; i < upgradeElements.Length; i++)
        {
            upgradeElements[i].RefreshUpgradeElement();
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
        PlayerData playerData =  GameManager.Instance.GetPlayerData();
        playerData.SetUpgrade(upgradeElement);
        RefreshUpgrades();
    }

    public void Save()
    {
        SaveSystem.SaveGame();
    }

}