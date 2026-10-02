using System;
using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.Core;
using UnityEngine;
[Serializable]
public enum popupType
{
    error, message
}

public class PopupDialogManager : MonoBehaviour
{
    [SerializeField]
    [DictionaryDisplay]
    public Dictionary<popupType, MessageDialog> 
        popupDialogMessageDict = new Dictionary<popupType, MessageDialog>();

    private IEventService eventService;

    private void Start()
    {
        eventService = GameContext.Get<IEventService>();

        eventService.Subscribe<ErrorDialogEvent>(Show);
    }

    public void Show(ErrorDialogEvent payload)
    {
        if (popupDialogMessageDict.TryGetValue(payload.Type, out MessageDialog dialog))
        {
            dialog.Show();
            dialog.SetText(payload.Message);
        }
    }
}

