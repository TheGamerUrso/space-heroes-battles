using EasyMobile;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasyMobileInitializer : MonoBehaviour
{
    private void Awake()
    {
        if (!RuntimeManager.IsInitialized())
            RuntimeManager.Init();
    }
}
