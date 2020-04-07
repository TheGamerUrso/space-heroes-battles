using TMPro;
using UnityEngine;

public class CoinWidget : MonoBehaviour
{
    public TextMeshProUGUI CoinText;

    private void Update()
    {
        PlayerData playerData = DataController.Instance.GetPlayerData();
        CoinText.text = "" + playerData.Coins;
    }
}
