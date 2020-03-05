using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

[RequireComponent(typeof(CanvasGroup))]
public class UIView : MonoBehaviour
{
 
   
    private CanvasGroup canvasGroup;
    public bool ViewIsActive
    {
        get
        {
            return canvasGroup.alpha == 1 ? true : false;
        }
    }
    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    public void Open()
    {
        Debug.Log(gameObject.name + " Open");
        canvasGroup.DOFade(1, 1).OnComplete(() =>
        {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        });
    }
    public void Close()
    {
        Debug.Log(gameObject.name + " Close");
        if (canvasGroup != null)
        {
            canvasGroup.interactable = false;
            canvasGroup.DOFade(0, 1).OnComplete(() =>
            {
                canvasGroup.blocksRaycasts = false;
            });
        }
        else
        {
            Debug.Log("Error" + gameObject.name);
        }
    }
}
