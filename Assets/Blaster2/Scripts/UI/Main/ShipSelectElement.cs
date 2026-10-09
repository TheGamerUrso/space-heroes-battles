using System;
using TheGamerUrso.Core;
using TMPro;
using UnityEditor.MPE;
using UnityEngine;
using UnityEngine.UI;

public class ShipSelectElement : MonoBehaviour
{
    [SerializeField] private ShipSelect shipSelect;
    [SerializeField] private int ID;
    [SerializeField] private ShipSelectData shipSelectData;
    private bool Locked;
    [SerializeField] private Button PurchaseButton;
    [SerializeField] private Image Icon;
    [SerializeField] private Image LockImage;
    [SerializeField] private TextMeshProUGUI CostText;
    private PlayerData playerData;
    private IDataService dataService;
    private INotificationService notificationService;

    private void Awake()
    {
        dataService = GameContext.Get<IDataService>();
        notificationService = GameContext.Get<INotificationService>();
    }

    private void Start()
    {
        Icon.sprite = shipSelectData.Icon;
        CostText.text = shipSelectData.Cost.ToString();

        playerData = dataService.GetPlayerData();

        if (playerData.UnlockedHeroes[ID] == 0)
        {
            Lock();
        }
        else if (playerData.UnlockedHeroes[ID] == 1)
        {
            Unlock();
        }

    }

    private void Update()
    {
        RefreshElement();
    }

    public void RefreshElement()
    {
        int coins = playerData.playerEconomyData.Coins;

        if (coins >= shipSelectData.Cost)
        {
            CostText.color = Color.green;
        }
        else
        {
            CostText.color = Color.red;
        }
        CostText.text = "" + shipSelectData.Cost;
    }


    public void Lock()
    {
        Locked = true;
        LockImage.gameObject.SetActive(Locked);
        CostText.gameObject.SetActive(true);
    }

    public void Unlock()
    {
        Locked = false;
        LockImage.gameObject.SetActive(Locked);
        CostText.gameObject.SetActive(false);
    }

    public void Purchase()
    {
        PlayerData playerData = dataService.GetPlayerData();
        if (playerData.playerEconomyData.Coins >= shipSelectData.Cost)
        {

            notificationService?.Show(new ErrorDialogEvent()
            {
                Type = popupType.message,
                Message = "Unlocked new Hero",
                AutoClose = true
            });

            playerData.playerEconomyData.Coins -= shipSelectData.Cost;
            playerData.UnlockedHeroes[ID] = 1;
            shipSelect.SelectShip(ID);
            Unlock();

            int num = 0;
            for (int i = 0; i < playerData.UnlockedHeroes.Length; i++)
            {
                if (playerData.UnlockedHeroes[i] == 1)
                {
                    num++;
                }
            }
        }
        else
        {
            notificationService?.Show(new ErrorDialogEvent()
            {
                Type = popupType.message,
                Message = "Insufficient funds",
                AutoClose = true
            });
        }
    }

    public void SelectShip()
    {
        if (Locked)
        {
            Purchase();
        }
        else
        {
            shipSelect.SelectShip(ID);
        }
    }
}