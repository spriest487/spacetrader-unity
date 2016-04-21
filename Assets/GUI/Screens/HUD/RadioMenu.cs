using UnityEngine;
using System.Collections.Generic;
using System;

public enum RadioMessageType
{
    Greeting
}

public struct RadioMessage
{
    private readonly Ship source;
    private RadioMessageType messageType;

    public Ship Source { get { return source; } }
    public RadioMessageType MessageType { get { return messageType; } }

    public RadioMessage(Ship source, RadioMessageType messageType)
    {
        this.source = source;
        this.messageType = messageType;
    }
}

public class RadioMenu : MonoBehaviour
{
    [SerializeField]
    private Transform content;

    void Start()
    {

    }

    public void ShowRadioMenu()
    {
        content.gameObject.SetActive(true);
    }

    public void Cancel()
    {
        content.gameObject.SetActive(false);
    }

    public void SendRadioBroadcast(string messageName)
    {
        var message = (RadioMessageType) Enum.Parse(typeof(RadioMessageType), messageName);
        var source = PlayerShip.LocalPlayer.Ship;

        var target = source.Target? source.Target.GetComponent<Ship>() : null;

        source.SendRadioMessage(message, target);

        Cancel();
    }
}

[Serializable]
public class RadioMenuItem
{
    [SerializeField]
    private List<RadioMenuItem> children;

    public RadioMenuItem()
    {
        children = new List<RadioMenuItem>();
    }
}
