using UnityEngine;
using UnityEngine.UI;

public class BaseHealthWidget : MonoBehaviour
{
    public float MaxHealth { get; private set; }
    public float CurrentHealth { get; private set; }

    public GameObject Target;
    protected Color currentColor;

    [SerializeField] protected Color RedColor = Color.red;
    [SerializeField] protected Color GreenColor = Color.green;

    [SerializeField] protected GameObject HealthbarCanvas;
    [SerializeField] protected GameObject HealthBarTransform;

    [SerializeField] protected Image HealthBarImage;
    [SerializeField] protected Image ActualHealthBarImage;
    [SerializeField] protected Image ShieldBarImage;

    public float lerpSpeed;

    [SerializeField] protected Vector3 offset;

    public virtual void Setup(IDamagable ship, bool follow = true) { }
    public virtual void Setup(IDamagable ship)
    {
        MonoBehaviour go = ship as MonoBehaviour;
        ship.OnHealthChanged += UpdateHealthBar;
        Target = go.gameObject;
    }
    public virtual void Awake() { }

    public virtual void Start() { }
    public virtual void Update()
    {
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

    protected virtual void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        this.CurrentHealth = currentHealth;
        this.MaxHealth = maxHealth;
        HealthBarImage.fillAmount = CurrentHealth / MaxHealth;
        HealthBarImage.color = Color.Lerp(RedColor, GreenColor, HealthBarImage.fillAmount);
    }
}
