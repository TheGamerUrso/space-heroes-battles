using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMaterialOffset : MonoBehaviour
{
    public enum Move { Y, X, Both }
    public Move move;

    private string offsetKey = "_MainTex";
    [SerializeField] private float scrollSpeed = 0.5F;
    [SerializeField] private float xScrollSpeed = 0;

    private Renderer rend;

    [SerializeField] private float offset;
    [SerializeField] private float offXset;


    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        switch (move)
        {
            case Move.Y:

                offXset += xScrollSpeed * Time.deltaTime;

                break;
            case Move.X:
                offset += scrollSpeed * Time.deltaTime;

                break;
            case Move.Both:
                offset += scrollSpeed * Time.deltaTime;
                offXset += xScrollSpeed * Time.deltaTime;
                break;
            default:
                break;
        }


        rend.material.SetTextureOffset(offsetKey, new Vector2(offXset, offset));
    }
}

