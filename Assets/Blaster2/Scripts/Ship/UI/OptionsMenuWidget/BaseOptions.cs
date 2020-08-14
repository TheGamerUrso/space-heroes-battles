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

    public AudioMixerGroup MusicMixerGroup;
    public AudioMixerGroup SFXMixerGroup;

    public Slider MusicVolume;
    public Slider SFXVolume;

    protected TextMeshProUGUI AudioMute;
    protected TextMeshProUGUI AutoFire;
    protected GameObject Distance_Controls;

    protected RectTransform rectTransform;

    public virtual void OnEnable()
    {
       
    }


    public virtual void Start()
    {
        
    }

    public virtual void ExitAndSave()
    {
        SaveSystem.SaveGame();
    }
}
