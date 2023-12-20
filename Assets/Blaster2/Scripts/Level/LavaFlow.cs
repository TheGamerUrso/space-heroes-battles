using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaFlow : MonoBehaviour
{
    private string offsetKey = "_LavaTex";
    private string MainTextOffset = "_MainTex";
    public float scrollSpeed = 0.5F;
    public float xScrollSpeed = 0;
    public Renderer rend;

    private float offset;
    private float offXset;

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
            offXset += xScrollSpeed * Time.deltaTime;
        }
        else
        {
            offset = Mathf.Sin(Time.time * frequently) * magnitute;
            offXset = Mathf.Sin(Time.time * frequently) * magnitute;
        }
        rend.material.SetTextureOffset(offsetKey, new Vector2(offXset, offset));
        rend.material.SetTextureOffset(MainTextOffset, new Vector2(offXset, offset));
    }
}
