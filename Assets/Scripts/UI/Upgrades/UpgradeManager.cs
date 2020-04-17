using Boo.Lang;
using UnityEngine;

public class UpgradeManager : Singleton<UpgradeManager>
{
    public delegate void Purschase();
    public Purschase OnPurschase;

    public UpgradeElement[] upgradeElements;

    private PlayerData playerData;
    private PlayerShipData playerShipData;
    private ObjectiveData objectiveData;

    protected override void OnCleanup()
    {
        base.OnCleanup();
        OnPurschase = null;
    }

    public void Notify()
    {
        OnPurschase?.Invoke();
    }

    public void SubscribePurchasable(IPurchasable purchasable)
    {
        OnPurschase += purchasable.OnPurschase;
    }

    public void UnsubscribePurchasable(IPurchasable purchasable)
    {
        OnPurschase -= purchasable.OnPurschase;
    }

    private void Start()
    {
        playerData = GameManager.Instance.GetPlayerData();
        playerShipData = playerData.GetCurrentPlayerShipData();

        for (int i = 0; i < upgradeElements.Length; i++)
        {
            UpgradeElement upgradeElement = upgradeElements[i];
            upgradeElement.InitUpgradeElement(this);
            upgradeElement.OnPurchased += Purchase;
        }
    }

    public void Purchase(UpgradeElement upgradeElement)
    {
        objectiveData = playerData.GetOnGoingObjectiveById(ObjectiveType.spend);

        if (objectiveData != null)
        {
            var progress = objectiveData.progress + upgradeElement.Cost;
            objectiveData.UpdateProgress(progress);
        }

        if (upgradeElement.upgradeData.MaxLevel > 0)
        {
            playerData.SetUpgrade((int)(upgradeElement.upgradeData.upgradeType), upgradeElement.Level);
        }
        else if (upgradeElement.upgradeData.MaxLevel == 0)
        {
            playerData.SetUpgrade((int)(upgradeElement.upgradeData.upgradeType - 1), upgradeElement.Level);
        }

        Notify();


    }


}