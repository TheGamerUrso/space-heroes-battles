using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBackground : MonoBehaviour {
    public MeshRenderer meshRenderer;
    public Material[] material;


	void Start () {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = material[Random.Range(0, material.Length)];

    }
	
}
