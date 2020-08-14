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
        Appear();
    }

    public override void Update()
    {
        base.Update();
        if (Game.IsGameOver)
        {
            gameObject.SetActive(false);
        }
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
