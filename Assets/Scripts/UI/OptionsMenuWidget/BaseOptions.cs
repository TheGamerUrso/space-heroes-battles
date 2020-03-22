using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class BaseOptions : MonoBehaviour
{
    public delegate void OnOptionsChanged();
    public static event OnOptionsChanged OnOptionsRecieved;

    protected float[] Distances = { 3, 4, 5 };
    public AudioMixerGroup MusicMixerGroup;
    public AudioMixerGroup SFXMixerGroup;

    public Slider MusicVolume;
    public Slider SFXVolume;

    protected TextMeshProUGUI AudioMute;
    protected TextMeshProUGUI AutoFire;
    protected GameObject Distance_Controls;

    protected RectTransform rectTransform;
    public GameObject DistanceSelectionIndicator;
    public Button ShortButton;
    public Button MidButton;
    public Button LongButton;
    public void OnEnable()
    {
        OnOptionEnter();
    }


    private void Start()
    {
        InitializeOptions();
    }

    public virtual void OnOptionEnter()
    {
        if (rectTransform == null)
            rectTransform = DistanceSelectionIndicator.GetComponent<RectTransform>();

        InitializeOptions();
        //RefreshAutoFire();
        // RefreshGlobalMute();
        UpdateDistance();
        //SaveSystem.LoadGameSettings(gameObject);
        UpdateDistanceOptionSelection();
        UpdateAudioVolume();
    }

    public virtual void InitializeOptions()
    {
        InitializeAudioOptions();
        InitializeDistanceOptions();
    }

    public void UpdateAudioVolume()
    {
        PlayerData playerData = DataController.GetPlayerData();
        MusicVolume.value = playerData.MusicVolume;
        SFXVolume.value = playerData.SFXVolume;
    }
    public void InitializeAudioOptions()
    {
        UpdateAudioVolume();

        MusicVolume.onValueChanged.AddListener((value) =>
        {
            SetMusicVolume(value);
        });

        SFXVolume.onValueChanged.AddListener((value) =>
        {
            SetSFXVolume(value);
        });
    }
    public void InitializeDistanceOptions()
    {
        UpdateDistanceOptionSelection();

        ShortButton.onClick.AddListener(() =>
        {
            rectTransform.localPosition = ShortButton.transform.localPosition;
        });

        MidButton.onClick.AddListener(() =>
        {
            rectTransform.localPosition = MidButton.transform.localPosition;
        });

        LongButton.onClick.AddListener(() =>
        {
            rectTransform.localPosition = LongButton.transform.localPosition;
        });
    }
    public void UpdateDistanceOptionSelection()
    {
        PlayerData playerData = DataController.GetPlayerData();
        for (int i = 0; i < Distances.Length; i++)
        {
            if (Distances[i] == playerData.distance)
            {
                if (i == 0)
                {
                    rectTransform.localPosition = ShortButton.transform.localPosition;
                }
                else if (i == 1)
                {
                    rectTransform.localPosition = MidButton.transform.localPosition;
                }
                else if (i == 2)
                {
                    rectTransform.localPosition = LongButton.transform.localPosition;
                }
            }
        }

    }
    public void SetMusicVolume(float value)
    {
        PlayerData playerData = DataController.GetPlayerData();
        playerData.MusicVolume = value;

        AudioManager.SetMusicVolume(value);
    }

    public void SetSFXVolume(float value)
    {
        PlayerData playerData = DataController.GetPlayerData();
        playerData.SFXVolume = value;

        AudioManager.SetSoundVolume(value);
    }
    public void Mute()
    {
        PlayerData playerData = DataController.GetPlayerData();
        bool mute = playerData.mute;
        if (mute)
        {
            playerData.mute = false;
        }
        else if (mute == false)
        {
            playerData.mute = true;
        }
        RefreshGlobalMute();
    }
    public void ToggleAutoFire()
    {
        PlayerData playerData = DataController.GetPlayerData();
        bool autofire = playerData.AutoAttack;
        if (autofire)
        {
            playerData.AutoAttack = false;
        }
        else if (autofire == false)
        {
            playerData.AutoAttack = true;
        }

        RefreshAutoFire();
    }
    public void UpdateDistance()
    {
        PlayerData playerData = DataController.GetPlayerData();
        var distance = playerData.distance;
        if (distance == Distances[0])
        {
            rectTransform.localPosition = ShortButton.transform.localPosition;
        }
        else if (distance == Distances[1])
        {
            rectTransform.localPosition = MidButton.transform.localPosition;
        }
        else if (distance == Distances[2])
        {
            rectTransform.localPosition = LongButton.transform.localPosition;
        }
    }

    public void RefreshGlobalMute()
    {
        PlayerData playerData = DataController.GetPlayerData();
        bool mute = playerData.mute;
        if (mute)
        {
            AudioMute.text = "unMute";
        }
        else if (mute == false)
        {
            AudioMute.text = "Mute";
        }
    }
    public void RefreshAutoFire()
    {
        PlayerData playerData = DataController.GetPlayerData();
        bool autofire = playerData.AutoAttack;
        if (autofire)
        {
            AutoFire.text = "On";
        }
        else if (autofire == false)
        {
            AutoFire.text = "Off";
        }
    }
    public void SetDistance(int distance)
    {
        PlayerData playerData = DataController.GetPlayerData();

        playerData.distance = Distances[distance - 1];
        UpdateDistance();
    }

    public virtual void ExitAndSave()
    {
        SaveSystem.SavePlayerData();
    }
}
