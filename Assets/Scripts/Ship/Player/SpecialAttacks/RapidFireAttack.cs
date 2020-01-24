using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RapidFireAttack : SpecialAttack
{

    private GameManager gm;

    void Start()
    {
        gm = GameObject.FindObjectOfType<GameManager>();
      //  if (gm)
       // previousRapidFireValue = gm.playerProgression.Fire;
    }

}
