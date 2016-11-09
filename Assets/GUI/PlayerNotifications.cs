using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;

public enum NotificationCategory
{
    None,
    Chat,
    Error,
    GameMessage
}

public interface IPlayerNotification
{
    string Text { get; }
    NotificationCategory Category { get; }
    float Created { get; }
    Ship Source { get; }
}

public class PlayerNotifications
{
    private class PlayerNotification : IPlayerNotification
    {
        public string Text { get; set; }
        public NotificationCategory Category { get; set; }

        public float Created { get; set; }

        public Ship Source { get; set; }
    }

    public const int HistorySize = 10;
    
    private static LinkedList<PlayerNotification> messages = new LinkedList<PlayerNotification>();

    private static void Add(PlayerNotification notification)
    {
        notification.Created = Time.time;

        messages.AddLast(notification);
        if (messages.Count > HistorySize)
        {
            messages.RemoveFirst();
        }
    }

    public static IEnumerable<IPlayerNotification> GetNotifications(int count, NotificationCategory category)
    {
        var result = messages.Cast<IPlayerNotification>();
        if (category != NotificationCategory.None)
        {
            result = result.Where(m => m.Category == category);
        }
        if (count > 0)
        {
            result = result.Take(count);
        }

        return result;
    }

    public static void Chat(Ship source, string message)
    {
        Add(new PlayerNotification
        {
            Category = NotificationCategory.Chat,
            Source = source,
            Text = string.Format("[<b>{0}</b>]: {1}", source.name, message)
        });
    }

    public static void GameMessage(string message)
    {
        Add(new PlayerNotification
        {
            Category = NotificationCategory.GameMessage,
            Text = message,
        });
    }

    public static void Error(string message)
    {
        Add(new PlayerNotification
        {
            Category = NotificationCategory.Error,
            Text = message
        });
    }
    
    public static void Clear()
    {
        messages.Clear();
    }
}
