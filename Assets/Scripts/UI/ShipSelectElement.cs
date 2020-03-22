using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShipSelectElement : MonoBehaviour
{
    public int ID;
    public ShipSelectData shipSelectData;
    public bool Locked;
    public Button PurchaseButton;
    public Image Icon;
    public Image LockImage;
    public TextMeshProUGUI CostText;

    private void Start()
    {
        Icon.sprite = shipSelectData.Icon;
        CostText.text = shipSelectData.Cost.ToString();
        if(shipSelectData.Cost == 0)
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
        PlayerData playerData = DataController.GetPlayerData();
        int coins = playerData.Coins;
        if (coins >= shipSelectData.Cost)
        {
            CostText.color = Color.green;
        }
        else
        {
            CostText.color = Color.red;
        }
    }


    public void Lock()
    {
        Locked = true;
        LockImage.gameObject.SetActive(Locked);
        CostText.gameObject.SetActive(true);
    }

    public void SelectShip(int shipId)
    {
        GameEventSystem.Call(GameEventType.ShipSelect, shipId);
    }

    public void Unlock()
    {      
        Locked = false;
        LockImage.gameObject.SetActive(Locked);
        CostText.gameObject.SetActive(false);
    }

    public void Purchase()
    {
        PlayerData playerData = DataController.GetPlayerData();
        if (Locked)
        {
            if (playerData.Coins >= shipSelectData.Cost)
            {
                Popup.Show(Popup.popupType.message, "Unlocked new Hero", true);
                playerData.Coins -= shipSelectData.Cost;
                Unlock();
            }
            else
            {
                Popup.Show(Popup.popupType.message, "Not Enough Coins", true);
            }
        }
        else
        {
        }
    }
}