using TMPro;
using UnityEngine;

public class CoinWidget : MonoBehaviour
{
    public TextMeshProUGUI CoinText;
    private PlayerData playerData;

    private void OnDestroy()
    {
        Events.OnCoinValueChanged -= UpdateCoins;
    }

    private void Start()
    {
        playerData = PersistantData.GetPlayerData();
        Events.OnCoinValueChanged += UpdateCoins;
        UpdateCoins(playerData.Coins);
    }

    public void UpdateCoins(int coins)
    {
        CoinText.text = "" + coins;
    }
}
