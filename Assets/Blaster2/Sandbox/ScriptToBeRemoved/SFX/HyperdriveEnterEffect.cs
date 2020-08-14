
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HyperdriveEnterEffect : MonoBehaviour
{
    [SerializeField] protected GameObject shipPivot;
    [SerializeField] private Ease easeMode;
    [SerializeField] protected float speed;
    protected EnemyMove baseEnemyAI;
    protected Enemy baseEnemy;

    public virtual void Awake()
    {
        baseEnemyAI = GetComponent<EnemyMove>();
        baseEnemy = GetComponent<Enemy>();
    }
    private void OnDestroy()
    {
        DOTween.Clear();
    }

    public virtual void OnEnable()
    {
        baseEnemy.EnableColliders(false);
        baseEnemy.DisableAllWeapons();
        shipPivot.transform.localPosition = new Vector3(0, 0, -120);
        shipPivot.transform.DOLocalMoveZ(0, speed).SetEase(easeMode).OnComplete(() =>
        {
            baseEnemyAI.EnableMovement();
            baseEnemy.EnableWeaponById(0);
            if (baseEnemy.HealthBar != null)
            {
                baseEnemy.HealthBar.Show();
            }
            baseEnemy.EnableColliders(true);
        });
    }


    public virtual void OnActivated()
    {
        baseEnemy.EnableColliders(false);
        GetComponent<Enemy>().DisableAllWeapons();
        shipPivot.transform.localPosition = new Vector3(0, 0, -120);
        shipPivot.transform.DOLocalMoveZ(0, speed).SetEase(easeMode).OnComplete(() =>
        {
            baseEnemyAI.EnableMovement();
            baseEnemy.EnableWeaponById(0);
            if (baseEnemy.HealthBar != null)
            {
                baseEnemy.HealthBar.Show();
            }
     
        });
    }

}
