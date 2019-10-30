using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BackgroundTextureController : MonoBehaviour
{
    public Texture[] Backgrounds;
    private MeshRenderer rend;

    private void Start()
    {
        rend = GetComponent<MeshRenderer>();
        rend.material.SetTexture("_MainTex", Backgrounds[Random.Range(0, Backgrounds.Length)]);
    }

}

