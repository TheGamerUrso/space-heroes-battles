using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStatsDebug : MonoBehaviour
{
    public TextMeshProUGUI[] textStates;
    public PlayerData playerData;
    public PlayerShipData playerShipData;
    public PlayerShip playerShip;

    private void Start()
    {
        playerData = PersistantData.GetPlayerData();
        playerShipData = playerData.GetCurrentPlayerShipData();
    }

    public void Update()
    {
        if(Time.frameCount % 1 == 0)
        {
            textStates[0].text = "iLv: " + playerShipData.level;
            textStates[1].text = "HP: " + 25 * playerShipData.level;
            textStates[2].text = "xp: " + playerShipData.xp;
            textStates[3].text = "xpToLevel: " + playerShipData.xpToLevel;

            textStates[4].text = "Dmg: - ";
          
            if (playerShip != null) {
                textStates[4].text = "Dmg: " + playerShip.Damage;
                textStates[5].text = "FR: " + playerShip.FireRate;
            }
            else
            {
                playerShip = PlayerManager.GetPlayer();
            }
          

            textStates[6].text = "MD: " + playerShipData.MagnetDistance;
            textStates[7].text = "MP: " + playerShipData.MagnetPower;
            textStates[8].text = "SD: " + playerShipData.SuperDamage;
            textStates[9].text = "SC: " + playerShipData.SuperChargeTime;
            textStates[10].text = "Armor: " + playerShipData.HasArmorUpgrade;
            textStates[11].text = "Shield: " + playerShipData.HasShield;
        }
    }
}
