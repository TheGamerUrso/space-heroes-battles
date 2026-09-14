using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.Core;
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

    void OnEnable()
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
        var dataService = GameContext.Get<IDataService>();
        playerData = dataService.GetPlayerData();
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
        var dataService = GameContext.Get<IDataService>();
        PlayerData playerData = dataService.GetPlayerData();
        var audioService = GameContext.Get<IAudioService>();
        switch (audioType)
        {
            case AudioType.Music:
                playerData.MusicVolume = value;
                audioService.SetMusicVolume(value);
                break;
            case AudioType.SFX:
                playerData.SFXVolume = value;
                audioService.SetSoundVolume(value);
                break;
            default:
                break;
        }

    }
}
