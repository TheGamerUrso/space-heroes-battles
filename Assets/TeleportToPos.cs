using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportToPos : MonoBehaviour
{
    public GameObject TargetPos;

    private void OnTriggerEnter(Collider other)
    {
        other.transform.position =
           new Vector3(other.transform.position.x,
           other.transform.position.y, TargetPos.transform.position.z);

        other.gameObject.SetActive(false);

        other.gameObject.SetActive(true);
    }

}
