using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonRandomRotation : MonoBehaviour
{
    float timer = 2;
    Vector3 targetRot;

    private void Update()
    {
        timer -= Time.deltaTime;
        if(timer < 0)
        {
            timer = Random.Range(2, 4);
            targetRot = new Vector3(0, Random.Range(-10, 10), 0);
            transform.localEulerAngles = targetRot;
        }
    }
}
