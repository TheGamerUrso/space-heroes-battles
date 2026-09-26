using System;
using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.Core;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class PlayerPowerCircleUI : MonoBehaviour
{
    private PlayerShip playerShip;
    private PlayerData playerData;
    private PlayerShipData playerShipData;

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
        PowerUpLevelChanged(0);
        PowerPackCollected(0);
    }

    public void Setup(PlayerData playerData,PlayerShipData playerShipData)
    {
        this.playerData = playerData;
        this.playerShipData = playerShipData;
    }

    public void ActivateSpecial()
    {
        PowerBut.interactable = false;
        PowerUIActiveAnimator.SetBool(ReadyStringKey, PowerBut.interactable);
    }

    public void PowerUpLevelChanged(float playerPowerUp)
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

    public void PowerPackCollected(int collected)
    {
        RefreshWeaponIndicatorSprite();
    }

    public void RefreshWeaponIndicatorSprite()
    {
        if (playerData == null) return;

        var sprite = WeaponIndicatorSpritesNotActivated[0];
        var collecterUpgrade = playerData.PowerPackCollected;

        if (m_PowerUps != null)
        {
            if (m_PowerUps.fillAmount == 1)
            {
                sprite = WeaponIndicatorSpritesActivated[playerData.PowerPackCollected];
            }
            else if (collecterUpgrade >= WeaponIndicatorSpritesNotActivated.Length)
            {
                sprite = WeaponIndicatorSpritesNotActivated[playerData.PowerPackCollected];
            }
            else
            {
                sprite = WeaponIndicatorSpritesNotActivated[playerData.PowerPackCollected];
            }
        }
        if (WeaponIndicatorImage != null)
            WeaponIndicatorImage.sprite = sprite;
    }

}
