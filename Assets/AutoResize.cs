using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoResize : MonoBehaviour
{
    public float size;
    public Blaster blaster;
    public bool Active;

    private void Start()
    {
        blaster.AboutToShoot += callback;
    }

    public void callback(bool active)
    {
        if (active && !Active)
        {
            size = 0;
            Active = true;
        }

        if (!active)
        {
            size = 0;
            Active = false;
        }
    }

    void Update()
    {
        if (Active)
        {
            size += Time.deltaTime;
        }
        transform.localScale = new Vector3(size, size, size);
    }
}
