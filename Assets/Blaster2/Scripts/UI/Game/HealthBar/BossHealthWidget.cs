using PrimeTween;
using UnityEngine;

public class BossHealthWidget : EnemyHealthWidget
{
    protected override void Awake()
    {
        base.Awake();
        HealthBarTransform.transform.localScale = new Vector3(0, 1, 1);
        Show();
    }

    public override void Show()
    {
        base.Show();
        Tween.ScaleX(HealthBarTransform.transform, endValue: 1, 2, ease: Ease.Linear);
    }
}
