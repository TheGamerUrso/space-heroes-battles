using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMaterialOffset : MonoBehaviour
{
    public enum Move { Y, X, Both }
    public Move move;

    private string offsetKey = "_BaseMap";
    [SerializeField] private float speedX = 0.5F;
    [SerializeField] private float speedY = 0;

    private Renderer rend;

    [SerializeField] private float offsetX;
    [SerializeField] private float offsetY;


    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        switch (move)
        {
            case Move.Y:
                offsetY += speedY * Time.deltaTime;
                break;
            case Move.X:
                offsetX += speedX * Time.deltaTime;
                break;
            case Move.Both:
                offsetX += speedX * Time.deltaTime;
                offsetY += speedY * Time.deltaTime;
                break;
            default:
                break;
        }


        rend.material.SetTextureOffset(offsetKey, new Vector2(offsetX, offsetY));
    }
}

