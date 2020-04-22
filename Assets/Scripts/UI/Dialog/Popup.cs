using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Popup;

[System.Serializable]
public class PopupElement
{
    public string name;
    public popupType popupType;
    public GameObject popup;
}

public class Popup : MonoBehaviour
{
    public enum popupType
    {
        error,message
    }

    public PopupElement[] popupelements;
    public static Dictionary<popupType, GameObject> popups = new Dictionary<popupType, GameObject>();
    public void Awake()
    {
        for (int i = 0; i < popupelements.Length; i++)
        {
            popups.Add(popupelements[i].popupType, popupelements[i].popup);
        }
    }
    public static void Show(popupType popupType,string message = "",bool autoClose = false)
    {
        if (popups.ContainsKey(popupType))
        {
            popups[popupType].GetComponent<MessageDialog>().Enabled(true);
            popups[popupType].GetComponent<MessageDialog>().SetWidgetText(message);
            if (autoClose)
            {
                popups[popupType].GetComponent<MessageDialog>().AutoClose();
            }
        }
    }


}

