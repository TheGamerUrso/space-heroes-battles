using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShieldWidget : MonoBehaviour
{
    [SerializeField] private Image m_ShieldImage;

    public void ShieldEffect(bool value)
    {
        m_ShieldImage.gameObject.SetActive(value);
    }
}
