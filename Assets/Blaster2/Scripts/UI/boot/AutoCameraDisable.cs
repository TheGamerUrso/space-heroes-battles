using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoCameraDisable : MonoBehaviour
{
    public Camera cam;
    void Update()
    {
        cam.enabled = false;
        if (Camera.main == null)
        {
            cam.enabled = true;
        }   
    }
}
