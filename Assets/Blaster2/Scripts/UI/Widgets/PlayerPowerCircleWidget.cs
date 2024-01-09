using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPowerCircleWidget : MonoBehaviour
{
    [SerializeField] private PlayerShip player;
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

    // private int WeaponUpgradeCollected = 0;

    [Space(2)]
    [SerializeField] private Animator PowerUIActiveAnimator = null;

    [Space(2)]
    [SerializeField] private Sprite[] WeaponIndicatorSpritesActivated = null;

    [SerializeField] private Sprite[] WeaponIndicatorSpritesNotActivated;


    private void OnDestroy()
    {
        if (player != null)
        {
            Events.PowerUpLevelValueChanged -= PowerUpLevelChanged;
            Events.OnPowerPackCollected -= PowerPackCollected;
        }
    }
    private void Start()
    {
        playerData = PersistantData.GetPlayerData();
        playerShipData = playerData.GetCurrentPlayerShipData();

        PowerBut.onClick.AddListener(() =>
        {
            ActivateSpecial();

        });

        Events.PowerUpLevelValueChanged += PowerUpLevelChanged;
        Events.OnPowerPackCollected += PowerPackCollected;

        PowerUpLevelChanged(0);
        PowerPackCollected(0);
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
            PowerUIActiveAnimator.SetBool(ReadyStringKey, PowerBut.interactable);
        }
        else
        {
            PowerBut.interactable = false;
            PowerUIActiveAnimator.SetBool(ReadyStringKey, PowerBut.interactable);
        }

        m_PowerUps.fillAmount = playerPowerUp;

        RefreshWeaponIndicatorSprite();
    }
    public void PowerPackCollected(int collected)
    {
        RefreshWeaponIndicatorSprite();
    }

    public void RefreshWeaponIndicatorSprite()
    {
        var sprite = WeaponIndicatorSpritesNotActivated[0];
        var collecterUpgrade = playerData.PowerPackCollected;

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

        WeaponIndicatorImage.sprite = sprite;
    }

}
