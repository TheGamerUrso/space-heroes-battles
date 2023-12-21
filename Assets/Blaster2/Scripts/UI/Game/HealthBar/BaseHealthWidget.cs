using UnityEngine;
using UnityEngine.UI;

public class BaseHealthWidget : MonoBehaviour
{
    public float MaxHealth { get; protected set; }
    public float CurrentHealth { get; protected set; }
    public Ship ship;
    protected Color currentColor;

    [SerializeField] protected Color fullHealthColor = Color.green;
    [SerializeField] protected Color zeroHealthColor = Color.red;
    [SerializeField] protected GameObject HealthbarCanvas;
    [SerializeField] protected GameObject HealthBarTransform;

    [SerializeField] protected Image HealthBarImage;
    [SerializeField] protected Image ActualHealthBarImage;
    [SerializeField] protected Image ShieldBarImage;

    public float lerpSpeed = 1;

    [SerializeField] protected Vector3 offset;
   private void OnDestroy()
    {
        if(ship)
            ship.OnHealthChanged -= UpdateHealthBar;
    }
    public virtual void Setup(IDamagable damagable, bool follow = true) { }
    public virtual void Setup(IDamagable damagable)
    {
        MonoBehaviour mono = damagable as MonoBehaviour;
        ship = mono.GetComponent<Ship>();
        mono.GetComponent<Ship>().OnHealthChanged += UpdateHealthBar;
        UpdateHealthBar(ship.CurrentHealth, ship.MaxHealth);
    }
    public virtual void Awake() { }

    public virtual void Start(){}
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
        HealthBarImage.color = Color.Lerp(zeroHealthColor, fullHealthColor, HealthBarImage.fillAmount);
    }
}
