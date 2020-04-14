using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UpgradeElement : MonoBehaviour
{
    public UpgradeData upgradeData;
    public TextMeshProUGUI CostText;
    public TextMeshProUGUI NammeText;
    public Image UpgradeIcon;
    public int Cost;

    [Range(0, 10)]
    public int Level;

    public Image progressBar;

    [Range(0, 10)]
    public int currentUpgradeIndex;

    public float refreshUpgradeScreenDelay;

    public Image NotAvailableImage;
    public TextMeshProUGUI NotAvailbleText;

    private PlayerData playerData;
    private PlayerShipData playerShipData;
    private ObjectiveData objectiveData;

    private void OnEnable()
    {
        playerData = GameManager.Instance.GetPlayerData();
        playerShipData = playerData.GetCurrentPlayerShipData();
        objectiveData = playerData.GetOnGoingObjectiveById(ObjectiveType.spend);
        RefreshUpgradeElement();
    }

    public bool CheckAvailable(bool showError = false)
    {  
        if (upgradeData.MaxLevel > 0)
        {
            if (playerShipData.level >= upgradeData.LevelRequirementPerLevel[currentUpgradeIndex])
            {
                if(showError)
                Popup.Show(Popup.popupType.message, "Need Lv " + upgradeData.LevelRequirementPerLevel[currentUpgradeIndex].ToString());
                return true;
            }
        }
        else if (upgradeData.MaxLevel == 0)
        {
            if (playerShipData.Upgrades[(int)(upgradeData.upgradeType)-1] == 0)
            {
                if (showError)
                    Popup.Show(Popup.popupType.message, "Cannot Upgrade Anymore");
                return true;
            }
        }
        return false;
    }

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        playerData = GameManager.Instance.GetPlayerData();
        playerShipData = playerData.GetCurrentPlayerShipData();
        objectiveData = playerData.GetOnGoingObjectiveById(ObjectiveType.spend);

        if (upgradeData.MaxLevel > 0)
        {
            currentUpgradeIndex = playerShipData.Upgrades[(int)(upgradeData.upgradeType)];

            Cost = upgradeData.CostPerLevel[currentUpgradeIndex];
            NammeText.text = upgradeData.upgradeType.ToString();
            CostText.text = Cost.ToString();
            UpgradeIcon.sprite = upgradeData.sprite;

            if (currentUpgradeIndex <= upgradeData.LevelRequirementPerLevel.Length - 1)
            {
                CostText.text = Constants.UpgradeMaxedOut;
                NotAvailableImage.gameObject.SetActive(false);
                NotAvailbleText.gameObject.SetActive(false);
            }
        }
        else if (upgradeData.MaxLevel == 0)
        {
            currentUpgradeIndex = playerShipData.Upgrades[(int)(upgradeData.upgradeType)-1];
            Cost = upgradeData.Cost;
            NammeText.text = upgradeData.upgradeType.ToString();
            CostText.text = Cost.ToString();
            UpgradeIcon.sprite = upgradeData.sprite;

            if (currentUpgradeIndex == 1)
            {
                CostText.text = "Out of Stock";
                NotAvailableImage.gameObject.SetActive(false);
                NotAvailbleText.gameObject.SetActive(false);
            }
        }
    }
    public void Purshase()
    {
        if (CheckAvailable(true))
        {
            if (playerData.Coins < Cost)
            {
                Popup.Show(Popup.popupType.message,Constants.CannotAffordIt);
                return;
            }

            if (upgradeData.MaxLevel > 0)
            {
                if (upgradeData.CostPerLevel.Length - 1 <= currentUpgradeIndex)
                {
                    CostText.text = Constants.UpgradeMaxedOut;
                    return;
                }

                currentUpgradeIndex++;
            }else if(upgradeData.MaxLevel == 0)
            {
                currentUpgradeIndex = 1;
                CostText.text = Constants.OutOfStock;
            }

            objectiveData = playerData.GetOnGoingObjectiveById(ObjectiveType.spend);

            if (objectiveData != null)
            {
                var progress = objectiveData.progress + Cost;
                objectiveData.UpdateProgress(progress);
            }

            playerData.Coins -= Cost;

            GameEventSystem.Call(GameEventType.UpgradeBought, this);

            RefreshUpgradeElement();
        }
    }

    public void RefreshUpgradeElement()
    {
        playerShipData = playerData.GetCurrentPlayerShipData();

        NammeText.text = upgradeData.upgradeType.ToString();

        if (upgradeData.MaxLevel > 0)
        {
            currentUpgradeIndex = playerShipData.Upgrades[(int)(upgradeData.upgradeType)];
            Cost = upgradeData.CostPerLevel[currentUpgradeIndex];

            progressBar.fillAmount = (float)currentUpgradeIndex / 10;

        }else if(upgradeData.MaxLevel == 0)
        {
            currentUpgradeIndex = playerShipData.Upgrades[((int)(upgradeData.upgradeType) - 1)];
            Cost = upgradeData.Cost; 
            progressBar.fillAmount = (float)currentUpgradeIndex;
        }

        CostText.text = Cost.ToString();
        refreshUpgradeScreenDelay = 1;


        if (Cost > playerData.Coins)
        {
            CostText.color = Color.red;
        }
        else
        {
            CostText.color = Color.white;
        }

        if (CheckAvailable())
        {
            NotAvailableImage.gameObject.SetActive(false);
            NotAvailbleText.gameObject.SetActive(false);
        }
        else if (!CheckAvailable())
        {
            NotAvailableImage.gameObject.SetActive(true);
            NotAvailbleText.gameObject.SetActive(true);
            if (upgradeData.MaxLevel > 0)
            {
                NotAvailbleText.text = Constants.UnlockedAtLvl + upgradeData.LevelRequirementPerLevel[currentUpgradeIndex];
            }
        }

        if (currentUpgradeIndex == 1 && upgradeData.MaxLevel == 0)
        {
            CostText.text = Constants.OutOfStock;
            NotAvailableImage.gameObject.SetActive(false);
            NotAvailbleText.gameObject.SetActive(false);
        }
        else if(upgradeData.MaxLevel > 0)
        {
            if (upgradeData.CostPerLevel.Length - 1 <= currentUpgradeIndex)
            {
                CostText.text = Constants.UpgradeMaxedOut;
                CostText.color = Color.white;
                NotAvailableImage.gameObject.SetActive(false);
                NotAvailbleText.gameObject.SetActive(false);
            }
        }
    }
}