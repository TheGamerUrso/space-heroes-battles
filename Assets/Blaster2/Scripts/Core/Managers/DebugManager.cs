using TheGamerUrso.Core;
using UnityEngine;

public class DebugManager : MonoBehaviour
{

    private GameController gameController;
    private IDataService dataService;

    [ContextMenu("Infinite Money")]
    public void GiveInfinityCoins()
    {
        if(dataService == null)
        {
            dataService = GameContext.Get<IDataService>();
        }
        dataService.GetPlayerData().UpdateCurrency(99999);
    }
}
