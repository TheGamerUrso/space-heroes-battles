using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMaterialOffset : MonoBehaviour {
    private string offsetKey = "_MainTex";
    [SerializeField] private float scrollSpeed = 0.5F;
    [SerializeField] private float xScrollSpeed = 0;
    
    private Renderer rend;

    [SerializeField] private float offset;
    [SerializeField] private float offXset;

    [SerializeField] private bool sinMove;
    [SerializeField] private float frequently;
    [SerializeField] private float magnitute;


    void Start()
    {
        rend = GetComponent<Renderer>();
    }
    void Update()
    {
        if (sinMove == false)
        {
            offset += scrollSpeed * Time.deltaTime;
            offXset += xScrollSpeed * Time.deltaTime;
        }
        else
        {
            offset = Mathf.Sin(Time.time * frequently) * magnitute;
            offXset = Mathf.Sin(Time.time * frequently) * magnitute;
        }
        rend.material.SetTextureOffset(offsetKey, new Vector2(offXset, offset));
    }
}

