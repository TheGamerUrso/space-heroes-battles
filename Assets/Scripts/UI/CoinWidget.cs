using TMPro;
using UnityEngine;

public class CoinWidget : MonoBehaviour
{
    public TextMeshProUGUI CoinText;

    private void LateUpdatae()
    {
        PlayerData playerData = GameManager.Instance.GetPlayerData();
        CoinText.text = "" + playerData.Coins;
    }
}
