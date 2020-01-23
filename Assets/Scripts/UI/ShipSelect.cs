using UnityEngine;

[System.Serializable]
public class ShipEventArgs : System.EventArgs
{
    public ShipSelectElement selectElement { get; set; }
    public ShipSelectData shipSelectData { get; set; }
}

public class ShipSelect : MonoBehaviour
{
    public delegate void OnShipSelect(object sender, ShipEventArgs e);

    public static OnShipSelect ShipSelected;

    public GameObject[] Ships;
    private int currentShip;
    public ShipSelectElement[] shipSelectElement;

    public GameObject UIUnlockButton;
    public GameObject UISelectButton;


    public void Initialize()
    {
        PlayerData playerData = DataController.GetPlayerData();
        foreach (GameObject item in Ships)
        {
            item.SetActive(false);
        }
        if (playerData.UnlockedHeroes.Length == 0)
        {
            playerData.UnlockedHeroes[0] = 1;
        }


        for (int i = 0; i < shipSelectElement.Length; i++)
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

        SelectShip(currentShip);
    }

    private void Start()
    {
        Initialize();
    }

    public void Unlock()
    {

        PlayerData playerData = DataController.GetPlayerData();

        if (shipSelectElement[currentShip] != null)
        {
            shipSelectElement[currentShip].Purchase();
            if (shipSelectElement[currentShip].Locked == false)
            {
                SelectShip(currentShip);
            }
            playerData.UnlockedHeroes[currentShip] = 1;
        }
    }

    public void SelectShip(int shipID)
    {

        PlayerData playerData = DataController.GetPlayerData();
        ShipEventArgs e = shipSelectElement[shipID].Select();
        currentShip = shipID;

        foreach (GameObject item in Ships)
        {
            item.SetActive(false);
        }

        Ships[currentShip].SetActive(true);

        GameManager.instance.CurrentHeroChoosen = currentShip;

        UISelectButton.SetActive(true);
        UIUnlockButton.SetActive(false);

        if (e.selectElement.Locked)
        {
            UISelectButton.SetActive(false);
            UIUnlockButton.SetActive(true);
        }
        else if (e.selectElement.Locked == false)
        {
            playerData.currentSelectedShip = currentShip;
        }
    }

    public void DoneSelect()
    {
        gameObject.SetActive(false);
    }
}