using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : BaseHealthUI
{
    [SerializeField] private TextMeshProUGUI HealthText = null;

    protected override void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        base.UpdateHealthBar(currentHealth, maxHealth);
        CurrentHealth -= 20;
        MaxHealth -= 20;
        HealthText.text = string.Format("{0}/{1}", Mathf.Round(currentHealth), maxHealth);
    }

}
