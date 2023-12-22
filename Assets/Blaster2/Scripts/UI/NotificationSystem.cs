using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notification
{
    public int Index;
    public string Name;
    public Sprite icon;
    public string Description;
}

public class NotificationSystem : MonoSingleton<NotificationSystem>
{
    private List<GameObject> notifications = new List<GameObject>();
    [SerializeField] private Transform content;
    [SerializeField] private GameObject NotificationElementPrefab;
 
    public void Add(Notification notification)
    {
        var notificationGO = Instantiate(NotificationElementPrefab, content, false);

        var ttl = 2 * (notifications.Count + 1);

        notificationGO.GetComponent<NotificationElement>().SetNotificationElement(notification, this, Remove, ttl);

        notifications.Add(notificationGO);
    }

    public void Remove(NotificationElement notification)
    {
        notifications.Remove(notification.gameObject);
        notification.Close();
    }

}
