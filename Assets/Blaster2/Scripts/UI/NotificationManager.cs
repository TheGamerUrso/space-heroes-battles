using System;
using System.Collections;
using System.Collections.Generic;
using TheGamerUrso.Core;
using UnityEngine;

[Serializable]
public enum popupType
{
    error, message
}

public class Notification
{
    public int Index;
    public string Name;
    public Sprite icon;
    public string Description;
}

public class NotificationManager : ServiceComponent<INotificationService>, INotificationService
{
    private List<GameObject> notifications = new List<GameObject>();
    [SerializeField] private Transform content;
    [SerializeField] private GameObject NotificationElementPrefab;

    [SerializeField]
    [DictionaryDisplay]
    public Dictionary<popupType, MessageDialog>
        popupDialogMessageDict = new Dictionary<popupType, MessageDialog>();
    private IEventService eventService;

    private void Start()
    {
        eventService = GameContext.Get<IEventService>();

        eventService.Subscribe<ErrorDialogEvent>(Show);
    }

    public void Show(ErrorDialogEvent payload)
    {
        if (popupDialogMessageDict.TryGetValue(payload.Type, out MessageDialog dialog))
        {
            dialog.Show();
            dialog.SetText(payload.Message);
        }
    }

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
