#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;

public class MessagePanelItem : MonoBehaviour
{
    [SerializeField]
    private Text messageText;

    public void Assign(string message)
    {
        messageText.text = message;
    }
}