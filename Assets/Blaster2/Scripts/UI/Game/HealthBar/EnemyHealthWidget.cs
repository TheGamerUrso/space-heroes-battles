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

    public override void Awake() => Hide();

    protected override void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (ship == null) return;
        Show();
        timer = duration;
        base.UpdateHealthBar(currentHealth, maxHealth);
    }

    public override void Setup(IDamagable damagable, bool follow = true)
    {
        Setup(damagable);
        Static = follow;
    }

    public void LateUpdate()
    {
        if (Static) return;
        if (ship == null) return;
        HealthBarTransform.transform.position = Camera.main.WorldToScreenPoint(ship.transform.position) + offset;
    }

    public override void Update()
    {
        base.Update();
        if (!AutoHide) return;


        if (HealthbarCanvas.activeSelf && timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                Hide();
            }
        }

        if (Game.IsGameOver)
        {
            gameObject.SetActive(false);
        }
    }

}
