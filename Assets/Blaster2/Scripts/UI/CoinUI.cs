using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.Core;
using TMPro;
using UnityEngine;

public class CoinUI : MonoBehaviour
{
    [SerializeField] private GameController gameController;
    [SerializeField] private TextMeshProUGUI PlayerCoinText;
    public GameObject panel;

    private void OnDestroy()
    {
        if(gameController != null)
            gameController.OnGameCoinsPickedValueChanged  -= UpdateCoins;
    }

    private void Start()
    {
        if (gameController != null)
            gameController.OnGameCoinsPickedValueChanged += UpdateCoins;

        UpdateCoins(0);
    }

    public void UpdateCoins(int coins)
    {
        panel.SetActive(true);
        PlayerCoinText.text = "" + coins;
    }
}
