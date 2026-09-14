using System;
using System.Collections;
using TheGamerUrso.Core;
using UnityEngine;

[System.Serializable]
public class ShipEventArgs : System.EventArgs
{
    public ShipSelectElement selectElement { get; set; }
    public ShipSelectData shipSelectData { get; set; }
}

public class ShipSelect : UIView
{
    public event Action OnShipSelected;
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
    private IDataService dataService;

    protected void Awake()
    {
        dataService = GameContext.Get<IDataService>();
        playerData = dataService.GetPlayerData();
    }

    protected void Start()
    {
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
        playerData.SetCurrentSelectShip(currentShipSelected);
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
            playerData.SetCurrentSelectShip(currentShipSelected);
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
        OnShipSelected?.Invoke();
    }
}