using UnityEngine;

public interface INotificationService 
{
    void Show(ErrorDialogEvent payload);
    void Add(Notification notification);
    void Remove(NotificationElement notification);
}
