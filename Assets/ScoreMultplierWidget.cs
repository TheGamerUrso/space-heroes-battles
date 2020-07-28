using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreMultplierWidget : MonoBehaviour
{
    public GameObject widgetPanel;
    public TextMeshProUGUI ScoreMultText;
    public float timer;

    public float duration = 1;
    public int vibriate = 10;
    public int elasticity = 1;

    public Ease ease;
    public Vector3 size;

    public void Animate()
    {
        widgetPanel.transform.DOPunchScale(size, duration, vibriate, elasticity).SetEase(ease);
    }
    public void SetText(string text)
    {
        Animate();
        timer = 2;
        widgetPanel.gameObject.SetActive(true);
        ScoreMultText.text = text;
    }

    private void Update()
    {
        if (widgetPanel.gameObject.activeInHierarchy)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                widgetPanel.gameObject.SetActive(false);
            }
        }
    }

}
