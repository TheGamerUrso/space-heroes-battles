using UnityEngine;

public class EnemyHealthWidget : BaseHealthUI
{
    public Enemy enemy;
    private float timer;
    public float duration;
    public bool Static;
    public bool AutoHide;

    protected virtual void Awake() => Hide();

    protected virtual void Start()
    {
        enemy.healthComponent.OnHealthChanged += HealthComponent_OnHealthChangedHandled;
    }

    private void OnDestroy()
    {
        enemy.healthComponent.OnHealthChanged -= HealthComponent_OnHealthChangedHandled;
    }

    private void HealthComponent_OnHealthChangedHandled(float currentHealth, float MaxHealth)
    {
        UpdateHealthBar(currentHealth, MaxHealth);
    }

    protected override void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (healthComponent == null) return;
        if (currentHealth <= 1) return;
        Show();
        timer = duration;
        base.UpdateHealthBar(currentHealth, maxHealth);
    }

    public override void Setup(IDamagable damagable, bool follow = true)
    {
        Setup(damagable);
        Static = !follow;
    }

    public void LateUpdate()
    {
        if (Static) return;
        if (healthComponent == null) return;
        HealthBarTransform.transform.position = Camera.main.WorldToScreenPoint(healthComponent.transform.position) + offset;
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
    }

}
