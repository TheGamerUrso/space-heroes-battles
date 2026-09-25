using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerXPUI : MonoBehaviour
{
    private PlayerShipData playerShipData;
    [SerializeField] private Slider XPBar = null;
    [SerializeField] private TextMeshProUGUI XPStatus;
    
    private void OnDestroy()
    { 
        if(playerShipData!=null)
        playerShipData.OnXPValueChanged -= UpdateXP;
    }

    public void Setup(PlayerShipData playerShipData)
    {
        this.playerShipData = playerShipData;
        playerShipData.OnXPValueChanged += UpdateXP;
        UpdateXP(playerShipData.level, playerShipData.xp, playerShipData.xpToLevel);
    }

    public void UpdateXP(int lvl, float xp, float xpToLevel)
    {
        XPBar.maxValue = xpToLevel;
        XPBar.value = xp;
        XPStatus.text = Mathf.Round(xp) + "/" + Mathf.Round(xpToLevel);
    }
}
