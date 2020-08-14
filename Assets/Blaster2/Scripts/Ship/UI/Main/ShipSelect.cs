using System.Collections;
using Doozy.Engine.UI;
using UnityEngine;

[System.Serializable]
public class ShipEventArgs : System.EventArgs
{
    public ShipSelectElement selectElement { get; set; }
    public ShipSelectData shipSelectData { get; set; }
}

public class ShipSelect : MonoSingleton<ShipSelect>
{

    [SerializeField] private Camera shipCameraPreview;
    [SerializeField] private GameObject[] Ships;

    [SerializeField] private ShipSelectElement[] shipSelectElement;

    [SerializeField] private GameObject UIUnlockButton;
    [SerializeField] private GameObject UISelectButton;

    [SerializeField] private LayerMask Ship1Layermask;
    [SerializeField] private LayerMask Ship2Layermask;
    [SerializeField] private LayerMask Ship3Layermask;

    private PlayerData playerData;
    private int currentShipSelected;

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        playerData = PersistantData.GetPlayerData();

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

        currentShipSelected = playerData.CurrrentSelectedShip;

        Events.OnShipSelect += SelectShip;

        RefreshShipTexture();
    }

    public void Unlock()
    {
        shipSelectElement[currentShipSelected].Purchase();
    }

    public void SelectShip(int shipID)
    {
        currentShipSelected = shipID;
        playerData.CurrrentSelectedShip = currentShipSelected;
        Refresh();
    }

    public void SetShipTexture(int shipSelected)
    {
        if (shipSelected == 0)
        {
            shipCameraPreview.cullingMask = Ship1Layermask;
        }
        else if (shipSelected == 1)
        {
            shipCameraPreview.cullingMask = Ship2Layermask;
        }
        else if (shipSelected == 2)
        {
            shipCameraPreview.cullingMask = Ship3Layermask;
        }
    }
    public void RefreshShipTexture()
    {
        if (currentShipSelected == 0)
        {
            shipCameraPreview.cullingMask = Ship1Layermask;
        }
        else if (currentShipSelected == 1)
        {
            shipCameraPreview.cullingMask = Ship2Layermask;
        }
        else if (currentShipSelected == 2)
        {
            shipCameraPreview.cullingMask = Ship3Layermask;
        }
    }

    public void Refresh()
    {
        StartCoroutine(DelayEnableSelectButton());

        RefreshShipTexture();

        if (playerData.UnlockedHeroes[currentShipSelected] == 0)
        {
            StartCoroutine(DelayEnableUnlockButton());
        }
        else if (playerData.UnlockedHeroes[currentShipSelected] == 1)
        {
            playerData.CurrrentSelectedShip = currentShipSelected;
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