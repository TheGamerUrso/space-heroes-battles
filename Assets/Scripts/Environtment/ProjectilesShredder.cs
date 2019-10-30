using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilesShredder : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == Constants.ENEMYPROJECTILETAG || other.tag == Constants.FRIENDLYPROJECTILETAG)
        {
            Destroy(other.gameObject, 2f);
        }
    }
}
