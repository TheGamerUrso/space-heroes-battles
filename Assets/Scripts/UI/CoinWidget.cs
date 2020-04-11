using TMPro;
using UnityEngine;

public class CoinWidget : MonoBehaviour
{
    public TextMeshProUGUI CoinText;
    private PlayerData playerData;

    private void OnDestroy()
    {
        playerData = GameManager.Instance.GetPlayerData();
        playerData.OnCoinValueChanged -= UpdateCoins;
    }

    private void Start()
    {
        playerData = GameManager.Instance.GetPlayerData();
        UpdateCoins(playerData.Coins);


        playerData.OnCoinValueChanged += UpdateCoins;
    }

    public void UpdateCoins(int coins)
    {
        CoinText.text = "" + coins;
    }
}
