using TMPro;
using UnityEngine;

public class CoinWidget : MonoBehaviour
{
    public TextMeshProUGUI CoinText;
    private PlayerData playerData;

    private void OnDestroy()
    {
        playerData.OnCoinValueChanged -= UpdateCoins;
    }

    private void Start()
    {
        playerData = GameManager.Instance.GetPlayerData();
        playerData.OnCoinValueChanged += UpdateCoins;
        UpdateCoins(playerData.Coins);
    }

    public void UpdateCoins(int coins)
    {
        CoinText.text = "" + coins;
    }
}
