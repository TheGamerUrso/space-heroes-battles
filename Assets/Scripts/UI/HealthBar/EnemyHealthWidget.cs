using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthWidget : BaseHealthWidget
{
    private float timer;
    public float duration;
    public bool Static;
    public bool AutoHide;


    // Start is called before the first frame update
    void Start()
    {
        if (AutoHide)
        {
            Hide();
        }
    }
    public override void Initiallize(Ship ship)
    {
        GameEventSystem.OnEnemyHit += OnDamageTaken;
    }

    public override void Refresh(IDestroyable user)
    {
        base.Refresh(user);
        if (AutoHide)
        {
            Hide();
        }

        HealthBarImage.fillAmount = user.CurrentHealth / user.MaxHealth;
        HealthBarImage.color = Color.Lerp(RedColor, GreenColor, HealthBarImage.fillAmount);

    }
    public override void OnDamageTaken(string id,object sender)
    {
        IDestroyable user = (IDestroyable)sender;
        MonoBehaviour userGO = user as MonoBehaviour;
        Ship ship = userGO.GetComponent<Ship>();

        if (Target == null)
        {
            Target = userGO.gameObject;
        }

        if (AutoHide)
        {
            Show();
        }

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

        timer = duration;

        HealthBarImage.fillAmount = user.CurrentHealth / user.MaxHealth;

        HealthBarImage.color = Color.Lerp(RedColor, GreenColor, HealthBarImage.fillAmount);
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
