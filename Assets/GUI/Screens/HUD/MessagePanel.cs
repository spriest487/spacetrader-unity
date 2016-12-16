#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class MessagePanel : MonoBehaviour
{
    private PooledList<MessagePanelItem, IPlayerNotification> notifications;

    [SerializeField]
    private Transform messageContent;

    [SerializeField]
    private MessagePanelItem messagePrefab;

    [SerializeField]
    private int notificationCount;

    [SerializeField]
    private float maxAge = 2;

    [SerializeField]
    private NotificationCategory notificationCategory;

    public int MessageCount
    {
        get { return notifications == null? 0 : notifications.Count; }
    }

    private void Update()
    {
        if (notifications == null)
        {
            notifications = new PooledList<MessagePanelItem, IPlayerNotification>(messageContent, messagePrefab);
        }

        var items = PlayerNotifications.GetNotifications(notificationCount, notificationCategory)
            .Where(n => n.Created > Time.time - maxAge);

        notifications.Refresh(items, (i, item, data) =>
            item.Assign(data.Text));
    }
}