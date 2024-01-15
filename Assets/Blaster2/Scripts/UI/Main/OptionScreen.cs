using System.Collections;
using Dan.Main;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionScreen : GooglePlayOptions
{
    public GameObject usernameChangeInputPanel;
    public TextMeshProUGUI username;
    public TMP_InputField usernameInput;
    public GameObject LoadingPanel;
    public override void Start()
    {
        base.Start();
        username.SetText(TheGamerUrso.Leaderboards.Instance.GetUsername());
    }

    public void SetUsername()
    {
        if (string.IsNullOrEmpty(usernameInput.text)) return;
        StartCoroutine(ChangeUsernameCoroutine());
    }

    IEnumerator ChangeUsernameCoroutine()
    {
        LoadingPanel.SetActive(true);
        TheGamerUrso.Leaderboards.myLeaderbosard.UpdateEntryUsername(
           usernameInput.text, OnEntryUsernameUpdated, ErrorCallback);
        yield return null;
    }

    public void OnEntryUsernameUpdated(bool sucess)
    {
        if (sucess)
        {
            username.text = TheGamerUrso.Leaderboards.Instance.GetUsername();
            usernameChangeInputPanel.SetActive(false);
        }
        LoadingPanel.SetActive(false);
    }

    public void ErrorCallback(string error)
    {
        LoadingPanel.SetActive(false);
        Debug.LogError(error);
    }

    public override void ExitAndSave()
    {
        base.ExitAndSave();
        MainMenuManager.Instance.Close();
    }

}