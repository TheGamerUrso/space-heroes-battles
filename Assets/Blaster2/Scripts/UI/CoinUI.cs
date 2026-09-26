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
    public float ttl;


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

    void Update()
    {
        ttl-=Time.deltaTime;
        if(ttl<=0)
        {
            ttl = 4;
            panel.SetActive(false);
        }
    }

    public void UpdateCoins(int coins)
    {
        ttl =4;
        panel.SetActive(true);
        PlayerCoinText.text = "" + coins;
    }
}
