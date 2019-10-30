using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPitch : MonoBehaviour
{
    private AudioSource source;
    [Range(0.5f,1)]
    public float minRange;
    [Range(1, 1.5f)]
    public float maxRange;

    void Start()
    {
        source = GetComponent<AudioSource>();
        source.pitch = UnityEngine.Random.Range(minRange, maxRange);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
