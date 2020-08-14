using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SurvivalHighscore : MonoBehaviour
{
    private Sequence effect;

    void Start()
    {
         effect = DOTween.Sequence();
        effect.Append(transform.DORotate(new Vector3(0, 0, 25), 1))
              .Append(transform.DORotate(new Vector3(0, 0, -25), 1))
              .Append(transform.DORotate(new Vector3(0, 0, 25), 1))
            .SetLoops(-1);
    }

}
