using TheGamerUrso.Core;
using UnityEngine;

public class DebugManager : MonoBehaviour
{

    private GameController gameController;
    private IDataService dataService;

    [Button("Infinite Money")]
    public void GiveInfinityCoins()
    {
        if(dataService == null)
        {
            dataService = GameContext.Get<IDataService>();
        }
        dataService.GetPlayerData().playerEconomyData.AddCurrency(99999);
    }

    [Button("Give XP")]
    public void GiveXP()
    {
        if (dataService == null)
        {
            dataService = GameContext.Get<IDataService>();
        }
        dataService.GetPlayerData().GetCurrentPlayerShipData().EarnXP(100);
    }

    [Button("Give Level")]
    public void GiveLevel()
    {
        if (dataService == null)
        {
            dataService = GameContext.Get<IDataService>();
        }
        dataService.GetPlayerData().GetCurrentPlayerShipData().EarnXP(dataService.GetPlayerData().GetCurrentPlayerShipData().xpToLevel);
    }
}
