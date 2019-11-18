using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayBossDeathExplosion : MonoBehaviour
{

    public GameObject[] Explosions;

    public float timer;
    public float timeBetweenExplosins;
    public int currentExplosion;

    private void Update()
    {
        if (timer > 0 && currentExplosion< Explosions.Length)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = timeBetweenExplosins;

                Explosions[currentExplosion].SetActive(true);
                currentExplosion++;
            }
        }
        else
        {
            Destroy(gameObject, 1f);
        }


    }
}
