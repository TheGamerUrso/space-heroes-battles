using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthWidget : EnemyHealthWidget
{
    private void OnEnable()
    {
        HealthBarTransform.transform.localScale = new Vector3(0, 1, 1);
    }

    public override void OnStart()
    {
        base.OnStart();       
    }
    public override void Show()
    {
        base.Show();
        Appear();
    }

    void Appear()
    {
        TweenParams tParams = new TweenParams().SetEase(Ease.Linear);
        HealthBarTransform.transform.DOScaleX(1, 2).SetAs(tParams);
    }

    public override void Setup(IDamagable ship, bool follow = false)
    {
        base.Setup(ship);
    }
}
