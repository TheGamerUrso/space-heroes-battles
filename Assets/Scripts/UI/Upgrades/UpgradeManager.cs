using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public UpgradeElement[] upgradeElements;

    private PlayerData playerData;
    private PlayerShipData playerShipData;


    private void OnDestroy()
    {
        GameEventSystem.OnUpgradeBought -= Refresh;
    }

    private void Start()
    {
        playerData = GameManager.Instance.GetPlayerData();
        playerShipData = playerData.GetCurrentPlayerShipData();
        GameEventSystem.OnUpgradeBought += Refresh;
    }

    public void RefreshUpgrades()
    {
        for (int i = 0; i < upgradeElements.Length; i++)
        {
            UpgradeElement upgradeElement = upgradeElements[i];
            upgradeElement.RefreshUpgradeElement();
        }
    }

    public void Refresh(UpgradeElement upgradeElement)
    {

        playerData.SetUpgrade(upgradeElement);
        RefreshUpgrades();
    }

    public void Save()
    {
        SaveSystem.SaveGame();
    }

}