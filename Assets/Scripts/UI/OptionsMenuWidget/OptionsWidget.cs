using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class OptionsWidget : MonoBehaviour
{
    public GameObject Window;


    public Slider m_MusicSlider;
    public Slider SoundSlider;
    public Button m_SaveAndExitButton;

    private Button m_DeleteSaveButton;

    void Start()
    {
        m_SaveAndExitButton.onClick.AddListener(() =>
        {
           // if (AudioManager.instance)
           //     AudioManager.instance.PlaySFX("back");
            SaveAndExitAction();
        });


        m_MusicSlider.onValueChanged.AddListener((float v) =>
        {
           // AudioManager.instance.UpdateVolume("Music", v);
        });

        SoundSlider.onValueChanged.AddListener((float v) =>
        {
           // AudioManager.instance.UpdateVolume("Sound", v);
        });

        //m_DeleteSaveButton.onClick.AddListener(() =>
        //{
        //    AudioManager.instance.PlaySound("click");
        //    PlayerPrefs.DeleteAll();

        //});


    }
    public void CloseWindow()
    {
        Window.SetActive(false);
    }

    public void OpenWindow()
    {
        Window.SetActive(true);
    }

    public void SaveAndExitAction()
    {
        CloseWindow();
    }


    public void LoadMusicSettings()
    {

    }

    public void OnSliderChange()
    {

    }
}
