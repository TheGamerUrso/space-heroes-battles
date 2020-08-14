using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class EnemyHealthWidget : BaseHealthWidget
{
    private float timer;
    public float duration;
    public bool Static;
    public bool AutoHide;

    public override void Awake()
    {
        Hide();
    }

    public override void Setup(IDamagable ship, bool follow = true)
    {
        if (Target == null)
        {
            MonoBehaviour go = ship as MonoBehaviour;
            ship.OnHealthChanged += UpdateHealthBar;
            Target = go.gameObject;
        }
    }

    protected override void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (Target == null)
        {
            return;
        }

        Show();

        timer = duration;

        base.UpdateHealthBar(currentHealth, maxHealth);
    }
    public void LateUpdate()
    {
        if (!Static)
        {
            if (Target != null)
            {
                HealthBarTransform.transform.position = Camera.main.WorldToScreenPoint(Target.transform.position) + offset;
            }
        }
    }

    public override void Update()
    {
        base.Update();

        if (AutoHide)
        {
            if (HealthbarCanvas.activeSelf && timer > 0)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    Hide();
                }
            }
        }
    }

}
