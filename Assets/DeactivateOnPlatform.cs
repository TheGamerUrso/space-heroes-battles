using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateOnPlatform : MonoBehaviour
{
    [SerializeField] private bool m_deactivateOnAndroid;
    [SerializeField] private bool m_deactivateOnStandalone;

    //--------------------------------------------------------------------------------------------------------------
    void Awake()
    {
#if UNITY_ANDROID
        if (m_deactivateOnAndroid)
        {
            gameObject.SetActive(false);
        }
#elif UNITY_STANDALONE
            if (m_deactivateOnStandalone)
            {
                gameObject.SetActive(false);
            }
#endif
    }
}
