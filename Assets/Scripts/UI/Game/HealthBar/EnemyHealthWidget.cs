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

    public override void Setup(IDamagable ship, bool follow = true)
    {
     
        if (Target == null || Target != ship)
        {
            MonoBehaviour monoGO = ship as MonoBehaviour;
            if (monoGO != null)
            {
                Ship shipGo = monoGO.GetComponent<Ship>();
                if (shipGo != null)
                {
                    shipGo.OnHealthChanged += UpdateHealthBar;
                    Target = ship;
                }
                else
                {
                    Punch shipAccesory = monoGO.GetComponent<Punch>();
                    if (shipAccesory != null)
                    {
                     //   Debug.Log("shipAccesory not found");
                        shipAccesory.OnHealthChanged += UpdateHealthBar;
                        Target = ship;
                    }
                }
            }
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
        Ship ship = Target as Ship;
        if (ship != null)
        {
            bool m_HasShield = ship.HasShieldModule();

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
            {
                MonoBehaviour go = Target as MonoBehaviour;
                if (go != null)
                    SetHealthBarPosition(go.transform);
            }
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
