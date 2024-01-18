using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinUI : MonoBehaviour
{
  private PlayerData playerData;
    [SerializeField] private TextMeshProUGUI PlayerCoinText;
    public GameObject panel;
    public float ttl;
    private void OnDestroy()
    {
        if(GameController.Instance!=null)
        GameController.Instance.OnGameCoinsPickedValueChanged  -= UpdateCoins;
    }

    private void Start()
    {
        playerData = PersistantData.GetPlayerData();
        GameController.Instance.OnGameCoinsPickedValueChanged += UpdateCoins;
        UpdateCoins(GameController.Instance.CoinPicked);
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
