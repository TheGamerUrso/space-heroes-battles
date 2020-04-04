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
    public Button ShortButton;
    public Button MidButton;
    public Button LongButton;
    public GameObject[] select;

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
        InitializeOptions();
        //RefreshAutoFire();
        // RefreshGlobalMute();
        UpdateDistance();
        //SaveSystem.LoadGameSettings(gameObject);
        UpdateAudioVolume();
    }

    public virtual void InitializeOptions()
    {
        InitializeAudioOptions();
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
        for (int i = 0; i < Distances.Length; i++)
        {
            if (Distances[i] == playerData.distance)
            {
                select[i].SetActive(true);
            }
            else
            {
                select[i].SetActive(false);
            }
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
        SaveSystem.SaveGame();
    }
}
