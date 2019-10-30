using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShipSelectElement : MonoBehaviour
{
    public event EventHandler<ShipEventArgs> ShipSelected;

    private int ID;
    public ShipSelectData shipSelectData;
    public bool Locked;
    public Button PurchaseButton;
    public Image Icon;
    public Image LockImage;
    public TextMeshProUGUI CostText;
    public int Cost;

    private void Start()
    {
        Cost = shipSelectData.Cost;
        Icon.sprite = shipSelectData.Icon;
        CostText.text = Cost.ToString();
        if(Cost == 0)
        {
            Unlock();
        }
        //if (shipSelectData.ID.Equals("Ship1"))
        //{
        //    DataController.instance.playerData.UnlockedHeroes[0] = 1;
        //    Unlock();
        //}
        //else
        //{
        //    Locked = true;
        //}

        //if (Locked)
        //{
        //    Lock.gameObject.SetActive(true);
        //}

        // PurchaseButton.onClick.AddListener(() =>
        // {
        // Purchase();
        //});
    }

    private void Update()
    {
        PlayerData playerData = DataController.GetPlayerData();
        int coins = playerData.Coins;
        if (coins >= Cost)
        {
            CostText.color = Color.green;
        }
        else
        {
            CostText.color = Color.red;
        }
    }

    public void RefreshElement()
    {
    }

    public ShipEventArgs Select()
    {
        ShipEventArgs e = new ShipEventArgs();
        e.selectElement = this;
        e.shipSelectData = shipSelectData;
        return e;
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
        PlayerData playerData = DataController.GetPlayerData();

        if (Locked)
        {
            int coins = playerData.Coins;

            if (coins >= Cost)
            {
                // Debug.Log("I have Enough For this Ship");
                playerData.Coins -= Cost;
                Unlock();
            }
            else
            {
                //Debug.Log("I do not have enough For this Ship");
            }
        }
        else
        {
        }
    }
}