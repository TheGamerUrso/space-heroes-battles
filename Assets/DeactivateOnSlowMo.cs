using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateOnSlowMo : MonoBehaviour
{   
    [SerializeField] private bool m_deactivateOnSlow;
        [SerializeField]private GameObject panel;
    void Update()
    {
        if (m_deactivateOnSlow)
        {
            panel.SetActive(!GameController.Instance.SlowMo);
        }
        else
        {
           panel.SetActive(GameController.Instance.SlowMo);
        }
    }
}
