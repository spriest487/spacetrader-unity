using UnityEngine;
using UnityEngine.UI;

public class MessagePanelItem : MonoBehaviour
{
    [SerializeField]
    private Text messageText;

    [SerializeField]
    private float created;

    public static MessagePanelItem CreateFromPrefab(MessagePanelItem prefab, string message)
    {
        var item = Instantiate(prefab);
        item.messageText.text = message;
        item.created = Time.time;
        return item;
    }
}