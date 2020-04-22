using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    public void PauseButton()
    {
        GuiManager.Instance.PauseButton();
    }
}
