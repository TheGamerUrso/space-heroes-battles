using System;
using System.Collections;
using System.Collections.Generic;
using Dan.Main;
using UnityEngine;

public class UpdateUsernamePanel : MonoBehaviour
{
    public Action<string> OnUsernameChanged;
    public TMPro.TMP_InputField usernameInput;
    public GameObject LoadingPanel;
    public TMPro.TextMeshProUGUI errorMessage;
    void OnDestroy()
    {
        if( TheGamerUrso.Leaderboards.Instance!=null)
        TheGamerUrso.Leaderboards.Instance.OnUsernameUpdated -= OnEntryUsernameUpdated;
    }
    void Start()
    {
        TheGamerUrso.Leaderboards.Instance.OnUsernameUpdated += OnEntryUsernameUpdated;
        PlayerData playerData = PersistantData.GetPlayerData();
        if (!String.IsNullOrEmpty(playerData.GetUsername()))
        {
            gameObject.SetActive(false);
        }
    }

    public void SetUsername()
    {
        if (string.IsNullOrEmpty(usernameInput.text)) return;
        StartCoroutine(ChangeUsernameCoroutine());
    }

    IEnumerator ChangeUsernameCoroutine()
    {     
        errorMessage.gameObject.SetActive(false);
        LoadingPanel.SetActive(true);
        TheGamerUrso.Leaderboards.Instance.SetUsername(usernameInput.text);
        yield return null;
    }

    public void OnEntryUsernameUpdated(bool sucess)
    {
        if (sucess)
        {        
            PlayerData playerData = PersistantData.GetPlayerData();   
            playerData.SetUsername(usernameInput.text);
            OnUsernameChanged?.Invoke(usernameInput.text);
            gameObject.SetActive(false);
        }
        LoadingPanel.SetActive(false);
    }

    public void ErrorCallback(string error)
    {
        LoadingPanel.SetActive(false);
        Debug.LogError(error);
        errorMessage.gameObject.SetActive(true);
        errorMessage.SetText("Username already taken");
    }
}
