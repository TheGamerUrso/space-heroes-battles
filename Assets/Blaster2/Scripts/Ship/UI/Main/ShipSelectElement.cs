using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShipSelectElement : MonoBehaviour
{
    public ShipSelect shipSelect;
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
        if (shipSelectData.Cost == 0)
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
        PlayerData playerData = PersistantData.GetPlayerData();

        int coins = playerData.Coins;

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
        PlayerData playerData = PersistantData.GetPlayerData();
        if (Locked)
        {
            if (playerData.Coins >= shipSelectData.Cost)
            {
                Popup.Show(Popup.popupType.message, "Unlocked new Hero", true);
                playerData.Coins -= shipSelectData.Cost;
                playerData.UnlockedHeroes[ID] = 1;
                SelectShip(ID);
                Unlock();

                int num = 0;
                for (int i = 0; i < playerData.UnlockedHeroes.Length; i++)
                {
                    if (playerData.UnlockedHeroes[i] == 1)
                    {
                        num++;
                    }
                }
                Debug.Log(num / 3);
                if (GooglePlayServicesManager.GetInitialized())
                    GooglePlayServicesManager.ReportAchivementProgress(EasyMobile.EM_GameServicesConstants.Achievement_Unlock_All_Heroes, num / 3);

            }
            else
            {
                Popup.Show(Popup.popupType.message, Constants.CannotAffordIt, true);
            }
        }
    }

    public void SelectShip(int shipID)
    {
        ShipSelect.Instance.SelectShip(shipID);
    }
}