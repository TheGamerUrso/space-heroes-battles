using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.Core;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class BaseOptions : UIView
{
    public delegate void OnOptionsChanged();
    public static event OnOptionsChanged OnOptionsRecieved;

    public AudioMixerGroup MusicMixerGroup;
    public AudioMixerGroup SFXMixerGroup;

    public Slider MusicVolume;
    public Slider SFXVolume;

    protected TextMeshProUGUI AudioMute;
    protected TextMeshProUGUI AutoFire;
    protected GameObject Distance_Controls;

    protected RectTransform rectTransform;
    protected IDataService dataService;

    protected virtual void Awake()
    {
        dataService = GameContext.Get<IDataService>();
    }
    public virtual void Start()
    {
        
    }

    public virtual void ExitAndSave()
    {
        SaveSystem.SaveGame();
    }
}
