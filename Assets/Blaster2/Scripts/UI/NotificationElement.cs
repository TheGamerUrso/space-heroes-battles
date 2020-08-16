using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NotificationElement : MonoBehaviour, IPointerClickHandler
{
    private bool IsClosing = false;
    private string hideStringKey = "Hide";
    private Action<NotificationElement> TimesUp;
    public Animator anim;
    [SerializeField] private TextMeshProUGUI DescriptionText;
    [SerializeField] private Image Icon;

    public Notification notification;
    public NotificationSystem notificationSystem;

    private float TTL = 2;

    public void SetNotificationElement(
        Notification notification,
        NotificationSystem notificationSystem, Action<NotificationElement> callback, int TTL = 2)
    {
        TimesUp = callback;
        this.notification = notification;
        this.notificationSystem = notificationSystem;

        if (notification.icon != null)
            this.Icon.sprite = notification.icon;

        DescriptionText.text = notification.Description;

        TTL = Mathf.Clamp(TTL, 2, 20);

        this.TTL = TTL;

    }

    private void Update()
    {
        if (TTL > 0)
        {
            TTL -= Time.deltaTime;
            if (TTL < 0)
            {
                TimesUp?.Invoke(this);
            }
        }
    }

    public void Close()
    {
        if (!IsClosing)
        {
            IsClosing = true;
            StartCoroutine(HideNotification());
        }
    }

    IEnumerator HideNotification()
    {
        int animIndex = Animator.StringToHash(hideStringKey);
        anim.SetTrigger(animIndex);
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject, .5f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        TimesUp?.Invoke(this);
    }
}
