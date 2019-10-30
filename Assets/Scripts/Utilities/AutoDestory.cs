using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestory : MonoBehaviour {
    public float ttl;
    private void Start()
    {
        Destroy(gameObject, ttl);
    }
}
