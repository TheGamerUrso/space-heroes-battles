using System;
using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.Core;
using UnityEngine;


public class PopupDialogManager : MonoBehaviour
{
    [SerializeField]
    [DictionaryDisplay]
    public Dictionary<popupType, MessageDialog> 
        popupDialogMessageDict = new Dictionary<popupType, MessageDialog>();
}

