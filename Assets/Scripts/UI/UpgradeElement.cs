using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class UpgradeElement : MonoBehaviour, IPointerClickHandler
{
    public Action<UpgradeElement> OnUpgradeBought;
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

    private PlayerManager playerManager;

    private void OnEnable()
    {
        RefreshUpgradeElement();
    }


    public bool CheckAvailable()
    {
        PlayerData playerData = DataController.GetPlayerData();
        if (upgradeData.MaxLevel > 0)
        {
            if (playerData.Level >= upgradeData.LevelRequirementPerLevel[currentUpgradeIndex])
            {
                return true;
            }
        }
        else if (upgradeData.MaxLevel == 0)
        {
            if (playerData.Upgrades[(int)(upgradeData.upgradeType)-1] == 0)
            {
                return true;
            }
        }
        return false;
    }

    private void Awake()
    {
        Initialize();
    }
    public void Initialize()
    {
        PlayerData playerData = DataController.GetPlayerData();
        if (upgradeData.MaxLevel > 0)
        {
            currentUpgradeIndex = playerData.Upgrades[(int)(upgradeData.upgradeType)];

            Cost = upgradeData.CostPerLevel[currentUpgradeIndex];
            NammeText.text = upgradeData.upgradeType.ToString();
            CostText.text = string.Format("{0}", Cost);
            UpgradeIcon.sprite = upgradeData.sprite;

            if (currentUpgradeIndex <= upgradeData.LevelRequirementPerLevel.Length - 1)
            {
                CostText.text = "Maxed";
                NotAvailableImage.gameObject.SetActive(false);
                NotAvailbleText.gameObject.SetActive(false);
            }
        }
        else if (upgradeData.MaxLevel == 0)
        {
            currentUpgradeIndex = playerData.Upgrades[(int)(upgradeData.upgradeType)-1];
            Cost = upgradeData.Cost;
            NammeText.text = upgradeData.upgradeType.ToString();
            CostText.text = string.Format("{0}", Cost);
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
        if (CheckAvailable())
        {
            PlayerData playerData = DataController.GetPlayerData();
            AudioManager.PlaySound("Click", 1);


            if (playerData.Coins < Cost)
            {
                return;
            }

            if (upgradeData.MaxLevel > 0)
            {
                if (upgradeData.CostPerLevel.Length - 1 <= currentUpgradeIndex)
                {
                    CostText.text = "Maxed";
                    return;
                }

                currentUpgradeIndex++;
            }else if(upgradeData.MaxLevel == 0)
            {
                currentUpgradeIndex = 1;
                CostText.text = "Out of Stock";
            }

            ObjectiveData objectiveData = playerData.GetOnGoingObjectiveById(ObjectiveType.spend);

            if (objectiveData != null)
            {
                var progress = objectiveData.progress + Cost;
                objectiveData.UpdateProgress(progress);
            }

            playerData.Coins -= Cost;

            OnUpgradeBought(this);

            RefreshUpgradeElement();
        }


        RefreshUpgradeElement();
    }

    public void RefreshUpgradeElement()
    {
        PlayerData playerData = DataController.GetPlayerData();

        NammeText.text = upgradeData.upgradeType.ToString();

        if (upgradeData.MaxLevel > 0)
        {
            currentUpgradeIndex = playerData.Upgrades[(int)(upgradeData.upgradeType)];
            Cost = upgradeData.CostPerLevel[currentUpgradeIndex];

            progressBar.fillAmount = (float)currentUpgradeIndex / 10;

        }else if(upgradeData.MaxLevel == 0)
        {
            currentUpgradeIndex = playerData.Upgrades[((int)(upgradeData.upgradeType) - 1)];
            Cost = upgradeData.Cost; 
            progressBar.fillAmount = (float)currentUpgradeIndex;
        }

        CostText.text = string.Format("{0}", Cost);
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
                NotAvailbleText.text = string.Format("Unlocked at lvl {0}", upgradeData.LevelRequirementPerLevel[currentUpgradeIndex]);
            }
        }

        if (currentUpgradeIndex == 1 && upgradeData.MaxLevel == 0)
        {
            CostText.text = "Out of Stock";
            NotAvailableImage.gameObject.SetActive(false);
            NotAvailbleText.gameObject.SetActive(false);
        }
        else if(upgradeData.MaxLevel > 0)
        {
            if (upgradeData.CostPerLevel.Length - 1 <= currentUpgradeIndex)
            {
                CostText.text = "Maxed";
                CostText.color = Color.white;
                NotAvailableImage.gameObject.SetActive(false);
                NotAvailbleText.gameObject.SetActive(false);
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject objectClicked = eventData.pointerPress;
        UpgradeElement upgradeElement = objectClicked.GetComponent<UpgradeElement>();
        //Debug.Log("Clicked" + upgradeElement.upgradeData.upgradeType);

        Purshase();
    }
}