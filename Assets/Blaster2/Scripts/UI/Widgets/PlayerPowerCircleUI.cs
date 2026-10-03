using System;
using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.Core;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class PlayerPowerCircleUI : MonoBehaviour
{
    private PlayerShip ship;

    [Space(2)]
    [SerializeField] private GameObject SuperWidget;

    private static string ReadyStringKey = "Ready";

    [SerializeField] private Button PowerBut;
    [SerializeField] private Image m_PowerUps;
    [SerializeField] private Image WeaponIndicatorImage;

    [Space(2)]
    [Range(1, 4)] private int CurrentWeapnType = 0;

    private int WeaponUpgradeCollected = 0;

    [Space(2)]
    [SerializeField] private Animator PowerUIActiveAnimator = null;

    [Space(2)]
    [SerializeField] private Sprite[] WeaponIndicatorSpritesActivated = null;

    [SerializeField] private Sprite[] WeaponIndicatorSpritesNotActivated;


    private void Start()
    {
        PowerBut.onClick.AddListener(() =>
        {
            ActivateSpecial();

        });
    }

    public void Setup(PlayerShip ship)
    {
        this.ship = ship;
        ChargePowerValueChangedHandled(ship.GetPlayerShipData().ChargePower);
        PlayerPowerCircleUI_OnPowerPackCollected(ship.GetPlayerShipData().PowerPackCollected);
        ship.GetPlayerShipData().OnPowerPackCollected += PlayerPowerCircleUI_OnPowerPackCollected;
        ship.GetPlayerShipData().OnSuperChargedValueChanged += ChargePowerValueChangedHandled;
    }

    private void PlayerPowerCircleUI_OnPowerPackCollected(float PowerPackCollected)
    {
        RefreshWeaponIndicatorSprite();
    }

    public void ActivateSpecial()
    {
        PowerBut.interactable = false;
        PowerUIActiveAnimator.SetBool(ReadyStringKey, PowerBut.interactable);
    }

    public void ChargePowerValueChangedHandled(float playerPowerUp)
    {
        if (playerPowerUp >= 1)
        {
            PowerBut.interactable = true;
            if (PowerUIActiveAnimator != null)
                PowerUIActiveAnimator.SetBool(ReadyStringKey, PowerBut.interactable);
        }
        else
        {
            PowerBut.interactable = false;
            if (PowerUIActiveAnimator != null)
                PowerUIActiveAnimator.SetBool(ReadyStringKey, PowerBut.interactable);
        }

        if (m_PowerUps != null)
            m_PowerUps.fillAmount = playerPowerUp;

        RefreshWeaponIndicatorSprite();
    }

    public void RefreshWeaponIndicatorSprite()
    {
        if (ship == null) return;

        var playerShipData = ship.GetPlayerShipData();
        if (playerShipData == null) return;

        var sprite = WeaponIndicatorSpritesNotActivated[0];
        var collecterUpgrade = playerShipData.PowerPackCollected;

        if (m_PowerUps != null)
        {
            if (m_PowerUps.fillAmount == 1)
            {
                sprite = WeaponIndicatorSpritesActivated[ship.GetPlayerShipData().PowerPackCollected];
            }
            else if (collecterUpgrade >= WeaponIndicatorSpritesNotActivated.Length)
            {
                sprite = WeaponIndicatorSpritesNotActivated[playerShipData.PowerPackCollected];
            }
            else
            {
                sprite = WeaponIndicatorSpritesNotActivated[playerShipData.PowerPackCollected];
            }
        }
        if (WeaponIndicatorImage != null)
            WeaponIndicatorImage.sprite = sprite;
    }

}
