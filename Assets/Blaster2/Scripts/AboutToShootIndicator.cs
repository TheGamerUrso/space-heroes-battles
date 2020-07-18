using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AboutToShootIndicator : MonoBehaviour
{
    public Blaster weaponScript;
    public bool playEffect;
    public float speed;
    public float size = 1;

    void Start()
    {
        weaponScript.AboutToShoot += AboutToShoot;
    }

    public void AboutToShoot(bool shooting)
    {
        if (shooting)
        {
            if (!playEffect)
            {
                playEffect = true;
                transform.DOScale(new Vector3(size, size, size), speed).OnComplete(() =>
                {    
                    transform.localScale = new Vector3(0, 0, 0);
                });
            }
        }
        else if (!shooting)
        {
            playEffect = false;
        }

    }
}
