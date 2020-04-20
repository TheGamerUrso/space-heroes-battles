using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class XPBarWidget : MonoBehaviour
{
    public PlayerShipData playerShipData;
    private PlayerData playerData;
    public TextMeshProUGUI PlayerXPText;
    public TextMeshProUGUI PlayerLevelText;
    private int currentSelectShip;

    private void Start()
    {
        PlayerData playerData =  PersistantData.GetPlayerData();
        playerShipData = playerData.GetCurrentPlayerShipData();
        var Level = playerShipData.level;
        var xp = playerShipData.xp;
        var xpToLevel = playerShipData.xpToLevel;
        currentSelectShip = playerData.currentSelectedShip;



    }

    public void UpdateXPBarWidget(int lvl,float xp ,float xpToLevel)
    {
        if (playerShipData.level >= playerShipData.MaxLevel)
        {
            PlayerLevelText.text = "" + playerShipData.level;
            PlayerXPText.text = "Maxed";
        }
        else
        {
            PlayerLevelText.text = "" + playerShipData.level;

            PlayerXPText.text = string.Format("{0}/{1}",
             playerShipData.xp,
                Mathf.Round(playerShipData.xpToLevel));
        }
    }
}