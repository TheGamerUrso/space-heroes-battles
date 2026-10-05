using UnityEngine;
using UnityEngine.UI;

public class BossDestroyablePartHealthUI : MonoBehaviour
{
    public float MaxHealth { get; protected set; }
    public float CurrentHealth { get; protected set; }
    [SerializeField] protected BossDestroyablePart bossDestroyable;
    protected Color currentColor;

    [SerializeField] protected Color fullHealthColor = Color.green;
    [SerializeField] protected Color zeroHealthColor = Color.red;
    [SerializeField] protected GameObject HealthbarCanvas;
    [SerializeField] protected GameObject HealthBarTransform;

    [SerializeField] protected Image HealthBarImage;
    [SerializeField] protected Image ActualHealthBarImage;
    [SerializeField] protected Image ShieldBarImage;
    public float lerpSpeed = 1;
    private float timer;
    [SerializeField] protected float duration;
    [SerializeField] protected bool AutoHide;

    private void OnDestroy()
    {
        if (bossDestroyable != null)
            bossDestroyable.OnHealthChanged -= UpdateHealthBar;
    }
    protected virtual void Awake() => Hide();

    protected virtual void Start()
    {
        if (bossDestroyable != null)
            bossDestroyable.OnHealthChanged += UpdateHealthBar;
    }
    protected void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (bossDestroyable == null) return;
        if (currentHealth <= 1) return;
        Show();

        timer = duration;
        this.CurrentHealth = currentHealth;
        this.MaxHealth = maxHealth;
        HealthBarImage.fillAmount = CurrentHealth / MaxHealth;
        HealthBarImage.color = Color.Lerp(zeroHealthColor, fullHealthColor, HealthBarImage.fillAmount);
    }

    public virtual void Update()
    {
        if (!AutoHide) return;
        if (HealthbarCanvas.activeSelf && timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                Hide();
            }
        }
        ActualHealthBarImage.fillAmount = Mathf.Lerp(ActualHealthBarImage.fillAmount, HealthBarImage.fillAmount, lerpSpeed);
    }

    public virtual void Show()
    {
        HealthbarCanvas.SetActive(true);
    }

    public void Hide()
    {
        HealthbarCanvas.SetActive(false);
    }

}
