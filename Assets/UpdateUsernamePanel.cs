using System;
using System.Collections;
using System.Collections.Generic;
using Dan.Main;
using UnityEngine;

public class UpdateUsernamePanel : MonoBehaviour
{
    public TMPro.TMP_InputField usernameInput;
    public GameObject LoadingPanel;
    public TMPro.TextMeshProUGUI errorMessage;
    void Start()
    {
        if (!String.IsNullOrEmpty(TheGamerUrso.Leaderboards.UserName))
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
        TheGamerUrso.Leaderboards.Instance.SetUsername(usernameInput.text,OnEntryUsernameUpdated);
        yield return null;
    }

    public void OnEntryUsernameUpdated(bool sucess)
    {
        if (sucess)
        {
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
