using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseHealthWidget : MonoBehaviour
{
    public float Health { get; }
    public float currentHealth { get; set; }

    public GameObject Target;
    private Color currentColor;
    public Color RedColor = Color.red;
    public Color GreenColor = Color.green;

    public GameObject HealthBarTransform;
    public Image HealthBarImage;
    public Image ShieldBarImage;


    public Vector3 offset;
    public virtual void OnDamageTaken(string id,object user) { }
    public virtual void Initiallize(Ship ship)
    {

    }

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

    public virtual void Refresh(IDestroyable user)
    {

    }
    public virtual void Tick()
    {

    }
    void UpdateHealthBar(Ship ship)
    {
        HealthBarImage.fillAmount = ship.GetHealthPresentage();
    }

    public void FolllowTarget()
    {
        if (Target)
        {
            SetHealthBarPosition(Target.transform);
            UpdateHealthBar(Target.GetComponent<Ship>());
        }
    }

    public void SetHealthBarPosition(Transform transform)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        HealthBarImage.transform.position = screenPos + offset;
    }
}
