using System.Collections;
using Doozy.Engine.UI;
using UnityEngine;

[System.Serializable]
public class ShipEventArgs : System.EventArgs
{
    public ShipSelectElement selectElement { get; set; }
    public ShipSelectData shipSelectData { get; set; }
}

public class ShipSelect : Singleton<ShipSelect>
{
    public Camera shipCameraPreview;
    public GameObject[] Ships;
    private int currentShip;
    public ShipSelectElement[] shipSelectElement;

    public GameObject UIUnlockButton;
    public GameObject UISelectButton;

    public LayerMask Ship1Layermask;
    public LayerMask Ship2Layermask;
    public LayerMask Ship3Layermask;

    public void Initialize()
    {
        PlayerData playerData = PersistantData.GetPlayerData();

        //foreach (GameObject item in Ships)
        //{
        //    item.SetActive(false);
        //}

        if (playerData.UnlockedHeroes.Length == 0)
        {
            playerData.UnlockedHeroes[0] = 1;
            shipSelectElement[0].Unlock();
        }


        for (int i = 1; i < shipSelectElement.Length; i++)
        {
            if (playerData.UnlockedHeroes[i] > 0)
            {
                shipSelectElement[i].Unlock();
            }
            else if (playerData.UnlockedHeroes[i] == 0)
            {
                shipSelectElement[i].Lock();
            }
        }

        currentShip = playerData.currentSelectedShip;

        GameEventSystem.OnShipSelect += SelectShip;

        playerData.CurrrentSelectedShip = currentShip;
        RefreshShipTexture();
    }

    public void Unlock()
    {
        shipSelectElement[currentShip].Purchase();

        PlayerData playerData = PersistantData.GetPlayerData();
        int num = 0;
        for (int i = 0; i < playerData.UnlockedHeroes.Length; i++)
        {
            if (playerData.UnlockedHeroes[i] == 1)
            {
                num++;
            }
        }

        if (GooglePlayServicesManager.Instance)
            GooglePlayServicesManager.Instance.ReportAchivementProgress(EasyMobile.EM_GameServicesConstants.Achievement_Unlock_All_Heroes, num);
    }

    public void SelectShip(int shipID)
    {
        currentShip = shipID;
        Refresh();
    }

    public void RefreshShipTexture()
    {
        if (currentShip == 0)
        {
            shipCameraPreview.cullingMask = Ship1Layermask;
        }
        else if (currentShip == 1)
        {
            shipCameraPreview.cullingMask = Ship2Layermask;
        }
        else if (currentShip == 2)
        {
            shipCameraPreview.cullingMask = Ship3Layermask;
        }
    }

    public void Refresh()
    {
        StartCoroutine(DelayEnableSelectButton());

        // foreach (GameObject item in Ships)
        // {
        //      item.SetActive(false);
        //  }

        RefreshShipTexture();

        //Ships[currentShip].SetActive(true);
        PlayerData playerData = PersistantData.GetPlayerData();
        if (playerData.UnlockedHeroes[currentShip] == 0)
        {
            StartCoroutine(DelayEnableUnlockButton());
        }
        else if (playerData.UnlockedHeroes[currentShip] == 1)
        {
            playerData.CurrrentSelectedShip = currentShip;
        }
    }

    IEnumerator DelayEnableSelectButton()
    {
        yield return new WaitForEndOfFrame();
        UISelectButton.SetActive(true);
        UIUnlockButton.SetActive(false);

    }

    IEnumerator DelayEnableUnlockButton()
    {
        yield return new WaitForEndOfFrame();
        UISelectButton.SetActive(false);
        UIUnlockButton.SetActive(true);
        
    }

    public void DoneSelect()
    {
        ScreenManager.Instance.Close();
    }
}