using TMPro;
using UnityEngine;

public class CoinWidget : MonoBehaviour
{
    private PlayerData playerData;
    [SerializeField] private TextMeshProUGUI PlayerCoinText;
    public GameObject panel;
    public float ttl;
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
        panel.SetActive(true);
        PlayerCoinText.text = "" + coins;
    }
}
