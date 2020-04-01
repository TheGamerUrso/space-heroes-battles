using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class XPBarWidget : MonoBehaviour
{
    public PlayerShip player;
    private PlayerData playerData;
    public TextMeshProUGUI PlayerXPText;
    public TextMeshProUGUI PlayerLevelText;
    private int currentSelectShip;

    private void Start()
    {       
        PlayerData playerData = DataController.GetPlayerData();
        var Level = playerData.Level;
        var xp = playerData.xp;
        var xpToLevel = playerData.xpToLevel;
        currentSelectShip = playerData.currentSelectedShip;

        player = PlayerManager.GetPlayerByID(playerData.currentSelectedShip).prefab;
    }

    public void UpdateXPBarWidget(int lvl,float xp ,float xpToLevel)
    {
        if (player.level >= player.MaxLevel)
        {
            PlayerLevelText.text = "" + player.level;
            PlayerXPText.text = "Maxed";
        }
        else
        {
            PlayerLevelText.text = "" + player.level;

            PlayerXPText.text = string.Format("{0}/{1}",
             player.xp,
                Mathf.Round(player.xpToLevel));
        }
    }
}