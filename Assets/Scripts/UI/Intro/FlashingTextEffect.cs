using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashingTextEffect : MonoBehaviour {
    public CanvasGroup m_CanvasGroup;
    private float alphaValue;
    public float m_FlashingSpeed;

    public void Enabled(bool value)
    {
        gameObject.SetActive(value);
    }

    private void OnEnable()
    {
        StartCoroutine(FlashingEffect());
    }

    private void Start()
    {
        alphaValue = m_CanvasGroup.alpha;
    }

    private void Update()
    {
        m_CanvasGroup.alpha = alphaValue;
    }

    IEnumerator FlashingEffect()
    {

        while (gameObject.activeSelf)
        {
            while (m_CanvasGroup.alpha < 1)
            {
                alphaValue+= m_FlashingSpeed * Time.deltaTime;
                yield return null;
            }
            yield return new WaitForSeconds(.1f);
            while (m_CanvasGroup.alpha > 0)
            {
                alphaValue-= m_FlashingSpeed * Time.deltaTime;
                yield return null;
            }

        }
    }
}
