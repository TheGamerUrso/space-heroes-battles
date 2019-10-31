using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetPropperCanvasScaler : MonoBehaviour
{
    void Awake()
    {
        var canvasScaler = GetComponent<CanvasScaler>();
        var ratio = Screen.height / (float)Screen.width;
        var rr = canvasScaler.referenceResolution;
        canvasScaler.matchWidthOrHeight = (ratio < rr.x / rr.y) ? 1 : 0;
    }

}
