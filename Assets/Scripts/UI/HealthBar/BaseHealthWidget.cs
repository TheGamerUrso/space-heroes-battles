using UnityEngine;
using UnityEngine.UI;

public class BaseHealthWidget : MonoBehaviour
{
    public float Health { get; }
    public float currentHealth { get; set; }

    protected Ship Target;
    protected Color currentColor;

    [SerializeField] protected Color RedColor = Color.red;
    [SerializeField] protected Color GreenColor = Color.green;


    [SerializeField] protected GameObject HealthBarTransform;

    [SerializeField] protected Image HealthBarImage;
    [SerializeField] protected Image ShieldBarImage;

    [SerializeField] protected Vector3 offset;

    public virtual void Setup(Ship ship, bool follow = true) { }
    public virtual void Setup(Ship ship)
    {
        ship.GetShipStatsSystem().HealthChanged += UpdateHealthBar;
        Target = ship;
    }

    private void Start()
    {
        OnStart();
    }

    public virtual void OnStart() { }

    private void Update()
    {
        Tick();
    }

    public void Show()
    {
        HealthBarTransform.SetActive(true);
    }

    public void Hide()
    {
        HealthBarTransform.SetActive(false);
    }

    public virtual void Tick() { }

    protected virtual void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        HealthBarImage.fillAmount = currentHealth / maxHealth;
        HealthBarImage.color = Color.Lerp(RedColor, GreenColor, HealthBarImage.fillAmount);
    }

    public void SetHealthBarPosition(Transform transform)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        HealthBarImage.transform.position = screenPos + offset;
    }
}
