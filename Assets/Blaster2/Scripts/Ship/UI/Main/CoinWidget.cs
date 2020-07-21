using TMPro;
using UnityEngine;

public class CoinWidget : MonoBehaviour
{
    private PlayerData playerData;
    [SerializeField] private TextMeshProUGUI PlayerCoinText;

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
        PlayerCoinText.text = "" + coins;
    }
}
