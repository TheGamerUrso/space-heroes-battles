using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthWidget : MonoBehaviour
{
    public PlayerShip player;

    [SerializeField] private Color fullHealthColor = Color.green;
    [SerializeField] private Color zeroHealthColor = Color.red;

    [SerializeField] private GameObject HealthWidget;
    [SerializeField] private TextMeshProUGUI HealthText = null;
    [SerializeField] private Image m_HealthImage = null;

    private float targetHealth = 0;
    private float maxTargetHealth = 0;

    private void OnDestroy()
    {
        if (player != null)
        {
            player.OnHealthChanged -= UpdatePlayerHealth;
        }
    }

    private void Start()
    {
        player.OnHealthChanged += UpdatePlayerHealth;

        UpdatePlayerHealth(player.currentHealth, player.MaxHealth);
    }

    public void UpdatePlayerHealth(float CurrentHealth, float MaxHealth)
    {
  
        HealthText.text = string.Format("{0}/{1}", Mathf.Round(CurrentHealth), MaxHealth);

        CurrentHealth -= 20;
        MaxHealth -= 20;

        m_HealthImage.fillAmount = (CurrentHealth / MaxHealth);
        maxTargetHealth = MaxHealth;
        targetHealth = CurrentHealth;

        m_HealthImage.color = Color.Lerp(zeroHealthColor, fullHealthColor, targetHealth / maxTargetHealth);
    }

}
