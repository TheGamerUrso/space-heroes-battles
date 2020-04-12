using UnityEngine;
using UnityEngine.UI;

public class BaseHealthWidget : MonoBehaviour
{
    public float Health { get; }
    public float currentHealth { get; set; }

    protected IDamagable Target;
    protected Color currentColor;

    [SerializeField] protected Color RedColor = Color.red;
    [SerializeField] protected Color GreenColor = Color.green;


    [SerializeField] protected GameObject HealthBarTransform;

    [SerializeField] protected Image HealthBarImage;
    [SerializeField] protected Image ShieldBarImage;

    [SerializeField] protected Vector3 offset;

    public virtual void Setup(IDamagable ship, bool follow = true) { }
    public virtual void Setup(IDamagable ship)
    {
        MonoBehaviour monoGO = ship as MonoBehaviour;
        if (monoGO != null)
        {
            Ship shipGo = monoGO.GetComponent<Ship>();
            shipGo.OnHealthChanged += UpdateHealthBar;
            Target = ship;
        }


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

    public virtual void Show()
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
