using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateOnSlowMo : MonoBehaviour
{   
    [SerializeField] private bool m_deactivateOnSlow;
        [SerializeField]private GameObject panel;
    public GameController gameController;
    void Update()
    {
        if (m_deactivateOnSlow)
        {
            panel.SetActive(!gameController.IsSlowMo);
        }
        else
        {
           panel.SetActive(gameController.IsSlowMo);
        }
    }
}
