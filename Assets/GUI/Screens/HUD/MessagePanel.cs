using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MessagePanel : MonoBehaviour
{
    const int MAX_MESSAGES = 20;

    [SerializeField]
    private List<MessagePanelItem> messages;

    [SerializeField]
    private Transform messageContent;

    [SerializeField]
    private MessagePanelItem messagePrefab;

    private void OnPlayerNotification(string message)
    {
        var newItem = MessagePanelItem.CreateFromPrefab(messagePrefab, message);
        newItem.transform.SetParent(messageContent, false);
        newItem.transform.SetAsLastSibling();
        messages.Add(newItem);
    }

    private void Update()
    {
        if (messages.Count > MAX_MESSAGES)
        {
            var removedCount = messages.Count - MAX_MESSAGES;

            messages.GetRange(MAX_MESSAGES, removedCount).ForEach(message => Destroy(message.gameObject));
            messages.RemoveRange(MAX_MESSAGES, removedCount);
        }
    }
}