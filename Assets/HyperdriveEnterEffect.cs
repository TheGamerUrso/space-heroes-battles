using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HyperdriveEnterEffect : MonoBehaviour
{
    public GameObject shipPivot;
    public Ease easeMode;
    public float speed;

    private void OnEnable()
    {
        GetComponent<BaseEnemy>().DisableAllWeapons();
        shipPivot.transform.localPosition = new Vector3(0, 0, -256);
        shipPivot.transform.DOLocalMoveZ(0, speed).SetEase(easeMode).OnComplete(() =>
        {
            GetComponent<BaseEnemy>().EnableWeaponById(0);
        });
    }

}
