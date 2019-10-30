using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMaterialOffset : MonoBehaviour {
    private string offsetKey = "_BaseMap";
    public float scrollSpeed = 0.5F;
    public Renderer rend;
    private float offset;

    public float frequently;
    public float magnitute;
    public bool sinMove;
    void Start()
    {
        rend = GetComponent<Renderer>();
    }
    void Update()
    {
        if (sinMove == false)
        {
            offset += scrollSpeed * Time.deltaTime;
        }
        else
        {
            offset = Mathf.Sin(Time.time * frequently) * magnitute;
        }
        rend.material.SetTextureOffset(offsetKey, new Vector2(0, offset));
    }
}

