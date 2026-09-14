using TheGamerUrso.Core;
using TMPro;
using UnityEngine;

public class CoinWidget : MonoBehaviour
{
    private PlayerData playerData;
    [SerializeField] private TextMeshProUGUI PlayerCoinText;

        private IDataService dataService;

    private void Awake()
    {
        dataService = GameContext.Get<IDataService>();
    }

    private void Start()
    {
        playerData = dataService.GetPlayerData();
        UpdateCoins(playerData.Coins);
    }

    public void UpdateCoins(int coins)
    {
        PlayerCoinText.text = "" + coins;
    }
}
