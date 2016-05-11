#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Collections;

public enum RadioMessageType
{
    Greeting
}

public struct RadioMessage
{
    private readonly Ship source;
    private RadioMessageType messageType;

    public Ship SourceShip { get { return source; } }
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

    private IEnumerator AnimateShowRadioMenu()
    {
        foreach (var entry in content.GetComponentsInChildren<RadioMenuEntry>())
        {
            entry.RefreshRadioMenu();
        }

        yield return null;

        content.gameObject.SetActive(true);
    }
    
    public void ShowRadioMenu()
    {
        StartCoroutine(AnimateShowRadioMenu());
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

    public void OnScreenActive()
    {
        Cancel();
    }
}


