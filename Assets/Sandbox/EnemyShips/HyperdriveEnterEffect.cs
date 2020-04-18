using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HyperdriveEnterEffect : MonoBehaviour
{
    public GameObject shipPivot;
    public Ease easeMode;
    public float speed;
    protected BaseEnemyAI baseEnemyAI;
    protected BaseEnemy baseEnemy;


    private void Awake()
    {
        OnAwake();
    }

    private void OnEnable()
    {
        OnActivated();
    }

    public virtual void OnAwake()
    {
        baseEnemyAI = GetComponent<BaseEnemyAI>();
        baseEnemy = GetComponent<BaseEnemy>();
    }

    public virtual void OnActivated()
    {
        baseEnemy.EnableColliders(false);
        GetComponent<BaseEnemy>().DisableAllWeapons();
        shipPivot.transform.localPosition = new Vector3(0, 0, -256);
        shipPivot.transform.DOLocalMoveZ(0, speed).SetEase(easeMode).OnComplete(() =>
        {
            baseEnemyAI.EnableMovement();
            baseEnemy.EnableWeaponById(0);
            baseEnemy.HealthBar.Show();
            baseEnemy.EnableColliders(true);
        });
    }

}
