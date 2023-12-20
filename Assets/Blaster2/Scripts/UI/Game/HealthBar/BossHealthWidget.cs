using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthWidget : EnemyHealthWidget
{
    public override void Awake()
    {
        base.Awake();
        HealthBarTransform.transform.localScale = new Vector3(0, 1, 1);
    }

    public override void Show()
    {
        base.Show();
        TweenParams tParams = new TweenParams().SetEase(Ease.Linear);
        HealthBarTransform.transform.DOScaleX(1, 2).SetAs(tParams);
    }
}
