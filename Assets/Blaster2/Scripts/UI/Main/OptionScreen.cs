using System.Collections;
using Dan.Main;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionScreen : BaseOptions
{
    public GameObject usernameChangeInputPanel;
    public TextMeshProUGUI username;
    void OnDestroy(){
         usernameChangeInputPanel.GetComponent<UpdateUsernamePanel>().OnUsernameChanged-=OnUsernameChanged;
   
    }
    public override void Start()
    {
        usernameChangeInputPanel.GetComponent<UpdateUsernamePanel>().OnUsernameChanged+=OnUsernameChanged;
    }
    void Update()
    {
        PlayerData playerData = PersistantData.GetPlayerData();
        username.SetText(playerData.GetUsername());
    }
    public void OnUsernameChanged(string username)
    {
            this.username.SetText(username);
    }
    public void ChangeUsername()
    {
        usernameChangeInputPanel.gameObject.SetActive(true);
    }

    public override void ExitAndSave()
    {
        base.ExitAndSave();
        MainMenuManager.Instance.Close();
    }

}