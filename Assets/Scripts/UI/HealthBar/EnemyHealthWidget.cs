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


    public override void OnStart()
    {
        base.OnStart();
        UpdateHealthBar(100, 100);
    }

    public override void Setup(Ship ship, bool follow = true)
    {
        if (Target == null || Target != ship)
        {
            Target = ship;
            ship.GetShipStatsSystem().HealthChanged += UpdateHealthBar;
            Static = follow;
        }
    }

    protected override void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (Target == null)
        {
            return;
        }

        if (AutoHide)
        {
            Show();
        }

        bool m_HasShield = Target.HasShieldModule();

        if (ShieldBarImage != null)
        {
            if (m_HasShield)
            {
                ShieldBarImage.fillAmount = 1;
            }
            else
            {
                ShieldBarImage.fillAmount = 0;
            }
        }

        timer = duration;

        base.UpdateHealthBar(currentHealth, maxHealth);
    }

    public override void Tick()
    {
        base.Tick();
        if (!Static)
        {
            if (Target != null)
                SetHealthBarPosition(Target.transform);
        }

        if (AutoHide)
        {
            if (HealthBarImage.gameObject.activeSelf && timer > 0)
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
