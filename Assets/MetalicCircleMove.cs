using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetalicCircleMove : MonoBehaviour
{
    public float speed;
    private void LateUpdate()
    {
        transform.Translate(new Vector3(0, 0, speed) * Time.deltaTime);
    }
}
