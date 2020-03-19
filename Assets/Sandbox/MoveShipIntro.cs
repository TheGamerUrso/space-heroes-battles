using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveShipIntro : MonoBehaviour
{
    public float speed;
    public Ease moveEase = Ease.Linear;
    
    private void Start()
    {
        MoveSide();
        MoveUP();
    }

    public void MoveSide()
    {
        transform.DOLocalMoveX(
        Random.Range(-1, 1), speed).OnComplete(MoveSide).SetEase(moveEase);
    }
    public void MoveUP()
    {
          transform.DOLocalMoveY(
         Random.Range(-.5f, .5f), speed).OnComplete(MoveUP).SetEase(moveEase);
    }
    private void OnDestroy()
    {
        DOTween.KillAll();
    }
}
