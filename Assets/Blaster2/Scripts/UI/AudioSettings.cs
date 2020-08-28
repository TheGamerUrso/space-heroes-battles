using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    public enum AudioType
    {
        Music,SFX
    }
    public AudioType audioType;

    private PlayerData playerData;
    public AudioMixerGroup MixerGroup;
    public Slider volumeSlider;

    private void OnEnable()
    {
        UpdateAudioVolume();
    }

    void Start()
    {
        UpdateAudioVolume();
        volumeSlider.onValueChanged.AddListener((value) =>
        {
            SetVolume(value);
        });
    }


    public void UpdateAudioVolume()
    {
        playerData = PersistantData.GetPlayerData();
        if (playerData != null)
        {
            switch (audioType)
            {
                case AudioType.Music:
                    volumeSlider.value = playerData.MusicVolume;
                    break;
                case AudioType.SFX:
                    volumeSlider.value = playerData.SFXVolume;
                    break;
                default:
                    break;
            }
        }
    }

    public void SetVolume(float value)
    {
        PlayerData playerData = PersistantData.GetPlayerData();

        switch (audioType)
        {
            case AudioType.Music:
                playerData.MusicVolume = value;
                AudioManager.SetMusicVolume(value);
                break;
            case AudioType.SFX:
                playerData.SFXVolume = value;
                AudioManager.SetSoundVolume(value);
                break;
            default:
                break;
        }

    }
}
