using System;
using TMPro;
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

    private void Start()
    {
        Icon.sprite = shipSelectData.Icon;
        CostText.text = shipSelectData.Cost.ToString();

        playerData = PersistantData.GetPlayerData();

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
                //Popup.Show(Popup.popupType.message, "Unlocked new Hero", true);
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

                AchievementSystem.instance.Report(12, num);

            }
            else
            {
                //Popup.Show(Popup.popupType.message, Constants.CannotAffordIt, true);
            }
        }
    }

    public void SelectShip(int shipID)
    {
        ShipSelect.Instance.SelectShip(shipID);
    }
}