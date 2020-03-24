using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthWidget : EnemyHealthWidget
{


    public override void OnStart()
    {
        base.OnStart();
        HealthBarTransform.transform.localScale = new Vector3(0, 1, 1);
        Appear();
    }
    void Appear()
    {
        TweenParams tParams = new TweenParams().SetDelay(1).SetEase(Ease.Linear);

        HealthBarTransform.transform.DOScaleX(1, 2).SetAs(tParams);

    }
    public override void Setup(Ship ship, bool follow = false)
    {
        base.Setup(ship);
    }
}
