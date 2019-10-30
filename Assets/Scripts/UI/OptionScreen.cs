using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionScreen : MonoBehaviour
{
    public delegate void OnOptionsChanged();

    public static event OnOptionsChanged OnOptionsRecieved;

    private float[] Distances = { 3, 4, 5 };
    public AudioMixerGroup MusicMixerGroup;
    public AudioMixerGroup SFXMixerGroup;

    private TextMeshProUGUI AudioMute;
    private TextMeshProUGUI AutoFire;
    private GameObject Distance_Controls;

    public Slider MusicVolume;

    public Slider SFXVolume;

    public Button ShortButton;
    public Button MidButton;
    public Button LongButton;
    private RectTransform rectTransform;
    public GameObject DistanceSelectionIndicator;

    private MainMenuManager mainMenuManager;


    public GameObject profile;
    public RawImage userIcon;
    public TMPro.TextMeshProUGUI username;


    public Sprite[] buttonSprites;
    public Button signInBut;

    public Button AchievementBut;
    public Button LeaderboardBut;

    Coroutine signupCoroutine;


    public void LogIn()
    {

        if (EasyMobile.GameServices.IsInitialized())
        {
            signInBut.GetComponent<Image>().sprite = buttonSprites[1];
            GooglePlayServicesManager.Instance.SignOut();
        }
        else
        {
            signInBut.GetComponent<Image>().sprite = buttonSprites[0];
            GooglePlayServicesManager.Instance.SignIn();
        }

        if (signupCoroutine != null)
        {
            StopCoroutine(signupCoroutine);
        }

        signupCoroutine = StartCoroutine(SignUp());
    }

    IEnumerator SignUp()
    {
        while (!GooglePlayServicesManager.isInitialized)
        {
            if (GooglePlayServicesManager.isInitialized)
            {
                signInBut.GetComponent<Image>().sprite = buttonSprites[0];
                GooglePlayServicesManager.Instance.SignOut();
            }
            else
            {
                signInBut.GetComponent<Image>().sprite = buttonSprites[1];
                GooglePlayServicesManager.Instance.SignIn();
            }
            yield return new WaitForSeconds(1);
        }
        if (GooglePlayServicesManager.isInitialized)
        {
            signInBut.GetComponent<Image>().sprite = buttonSprites[0];
        }
        else
        {
            signInBut.GetComponent<Image>().sprite = buttonSprites[1];
        }

    }

    private void OnEnable()
    {
        PlayerData playerData = DataController.GetPlayerData();
        if (mainMenuManager == null)
            mainMenuManager = MainMenuManager.instance;

          if (rectTransform == null) 
        rectTransform = DistanceSelectionIndicator.GetComponent<RectTransform>();

        //RefreshAutoFire();
        // RefreshGlobalMute();
        RefreshDistance();
        //SaveSystem.LoadGameSettings(gameObject);

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



        MusicVolume.value = playerData.MusicVolume;
        SFXVolume.value = playerData.SFXVolume;


        User user = GooglePlayServicesManager.Instance.GetUserInfo();

        if (EasyMobile.GameServices.IsInitialized() || user != null)
        {
            profile.SetActive(true);
        
            username.text = user.username;

            signInBut.GetComponent<Image>().sprite = buttonSprites[1];
        }
        else if (!EasyMobile.GameServices.IsInitialized() || user == null)
        {
            profile.SetActive(false);
            username.text = string.Format("User{0}", Random.Range(1000, 9999));
            signInBut.GetComponent<Image>().sprite = buttonSprites[0];
        }
    }

    private void Start()
    {
        PlayerData playerData = DataController.GetPlayerData();
        MusicVolume.value = playerData.MusicVolume;
        SFXVolume.value = playerData.SFXVolume;

        for (int i = 0; i < Distances.Length; i++)
        {
            if (Distances[i] == playerData.distance)
            {
                if (i == 0)
                {
                    rectTransform.localPosition = ShortButton.transform.localPosition;
                }else if(i == 1)
                {
                    rectTransform.localPosition = MidButton.transform.localPosition;
                }else if (i == 2)
                {
                    rectTransform.localPosition = LongButton.transform.localPosition;
                }
            }
        }


        MusicVolume.onValueChanged.AddListener((value) =>
        {
            SetMusicVolume(value);
        });

        SFXVolume.onValueChanged.AddListener((value) =>
        {
            SetSFXVolume(value);
        });

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

    public void SetMusicVolume(float value)
    {
        PlayerData playerData = DataController.GetPlayerData();
        playerData.MusicVolume = value;

        AudioManager.instance.SetMusicVolume(value);
    }

    public void SetSFXVolume(float value)
    {
        PlayerData playerData = DataController.GetPlayerData();
        playerData.SFXVolume = value;

        AudioManager.instance.SetSoundVolume(value);
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

    public void RefreshDistance()
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

        playerData.distance = Distances[distance-1];
    }

    public void ExitAndSave()
    {
        if (mainMenuManager)
        {
            mainMenuManager.ShowMessage("Settings Saved");
        }

        SimpleShipControls.UpdateOffset();


        SaveSystem.SavePlayerData();
    }


    public void ShowLeaderboards()
    {
        GooglePlayServicesManager.Instance.ShowLeaderboards();
    }

    public void ShowAchievement()
    {
        GooglePlayServicesManager.Instance.ShowAchievementa();
    }
}