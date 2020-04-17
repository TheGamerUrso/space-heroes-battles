using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MinerBossEnterFX : HyperdriveEnterEffect
{
    private Sequence mySequence;

    public override void OnActivated()
    {
        baseEnemy.EnableColliders(false);
        GetComponent<BaseEnemy>().DisableAllWeapons();

        shipPivot.transform.localPosition = new Vector3(0, 50, 200);
        mySequence = DOTween.Sequence();

        mySequence.Append(shipPivot.transform.DOLocalMove(new Vector3(0, 50, -80), speed))
          .Append(shipPivot.transform.DOLocalMove(new Vector3(0, 0, -80), speed/6))
          .Append(shipPivot.transform.DOLocalMove(new Vector3(0, 0, 0), speed/4)).OnComplete(() =>
          {
              baseEnemyAI.EnableMovement();
              baseEnemy.EnableWeaponById(0);
              baseEnemy.HealthBar.Show();
              baseEnemy.EnableColliders(true);
          });
    }
}
