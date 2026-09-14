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
    private void OnDestroy()
    {
        Events.OnCoinValueChanged -= UpdateCoins;
    }

    private void Start()
    {
        playerData = dataService.GetPlayerData();
        Events.OnCoinValueChanged += UpdateCoins;
        UpdateCoins(playerData.Coins);
    }

    public void UpdateCoins(int coins)
    {
        PlayerCoinText.text = "" + coins;
    }
}
