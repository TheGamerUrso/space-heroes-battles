using TheGamerUrso.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeElement : MonoBehaviour
{
    [SerializeField] private Button buyButton;
    [SerializeField] private Image progressbar;
    [SerializeField] private TextMeshProUGUI Name;
    [SerializeField] private TextMeshProUGUI Price;
    [SerializeField] private TextMeshProUGUI MessageText;
    [SerializeField] private Image Icon;

    [SerializeField] private GameObject NotAvailable;
    private IUpgradesService upgradesService;
    private IDataService dataService;

    private Color TextdefaultColor;
    private PlayerData playerData;
    private PlayerShipData playerShipData;
    [SerializeField] private UpgradeData upgradeData;
    private Upgrade upgrade;


    private void Start()
    {
        upgradesService = GameContext.Get<IUpgradesService>();
        dataService = GameContext.Get<IDataService>();

        playerData = dataService.GetPlayerData();
        playerShipData = playerData.GetCurrentPlayerShipData();
        playerData.OnCurrencyValueChanged += PlayerData_OnCurrencyValueChanged;
        playerData.OnCurrentShipSelectedValueChanged += PlayerData_OnCurrentShipSelectedValueChanged;
        playerData.GetCurrentPlayerShipData().OnLevelUp += PlayerShipData_OnLevelUp;

        upgradesService.OnUpgradeValueChanged += Upgrade_OnUpdateValueChanged;
        upgradesService.OnUpgradeErrorOccured += UpgradeManager_OnUpgradeErrorOccured;

        upgrade = upgradesService.GetUpgrade(upgradeData.upgradeType);
        Name.text = upgrade.GetUpgradeName();
        Icon.sprite = upgrade.upgradeData.sprite;
        TextdefaultColor = Price.color;

        Refresh();

    }

    private void PlayerData_OnCurrencyValueChanged(int obj)
    {
        Refresh();
    }

    private void OnDestroy()
    {
        upgradesService.OnUpgradeValueChanged -= Upgrade_OnUpdateValueChanged;
        upgradesService.OnUpgradeErrorOccured -= UpgradeManager_OnUpgradeErrorOccured;

        if (playerData == null) return;
        playerData.OnCurrentShipSelectedValueChanged -= PlayerData_OnCurrentShipSelectedValueChanged;

        playerData.GetCurrentPlayerShipData().OnLevelUp -= PlayerShipData_OnLevelUp;
    }

    private void PlayerData_OnCurrentShipSelectedValueChanged(int currentShip)
    {
        upgrade.SetUpgrade(playerShipData.Upgrades[(int)upgrade.upgradeData.upgradeType]);
    }
    private void PlayerShipData_OnLevelUp(int Level)
    {
        Refresh();
    }

    private void UpgradeManager_OnUpgradeErrorOccured(string message)
    {
        Warn(message); 
    }

    private void Upgrade_OnUpdateValueChanged(Upgrade obj)
    {
        Refresh();
    }

    public void Refresh()
    {
        Price.text = upgrade.GetCost().ToString();
        progressbar.fillAmount = upgrade.ProgressPresentage;

        MessageText.gameObject.SetActive(false);
        NotAvailable.SetActive(false);
        Price.color = TextdefaultColor;

        if (upgrade.IsMaxLevel())
        {
            buyButton.interactable = false;
            Warn(Constants.UpgradeMaxedOut);
        }
        else if (!upgrade.HasRequirementMet(playerShipData.Level))
        {
            NotAvailable.SetActive(true);
            Warn(Constants.UnlockedAtLvl + upgrade.GetLevelRequirment(upgrade.upgradeData.upgradeType));
            buyButton.interactable = false;
        }
        else if (!upgrade.CanAffordNextLevel(playerData.playerEconomyData.Coins))
        {
            Warn(Constants.OutOfStock);
            Price.color = Color.red;
            buyButton.interactable = false;
        }
        else
        {
            buyButton.interactable = true;
        }
    }


    public void BuyButton()
    {
        upgradesService.BuyUpgrade(upgradeData.upgradeType);
        Refresh();
    }

    public void Warn(string message)
    {
        MessageText.text = message;
        MessageText.gameObject.SetActive(true);
    }
}
