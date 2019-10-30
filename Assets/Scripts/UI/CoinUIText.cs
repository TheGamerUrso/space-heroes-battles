using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoinUIText : MonoBehaviour
{
    public TextMeshProUGUI CoinText;

    public GameObject ShopWindow;

    public void Update()
    {
        PlayerData playerData = DataController.GetPlayerData();
        int value = playerData.Coins;
        string formatedText = string.Format("{0}", value);
        UpdateText(formatedText);
    }

    public void UpdateText(string text)
    {
        CoinText.text = text;
    }

}

