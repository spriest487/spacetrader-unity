#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Collections;

public enum RadioMessageType
{
    Greeting,

    FollowMe,
    Attack,
    Wait,

    AcknowledgeOrder
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

    private void Send(string messageName, Ship target)
    {
        var message = (RadioMessageType)Enum.Parse(typeof(RadioMessageType), messageName);
        var source = PlayerShip.LocalPlayer.Ship;

        source.SendRadioMessage(message, target);
        Cancel();
    }

    public void SendGlobalBroadcast(string messageName)
    {
        Send(messageName, null);
    }

    public void SendFleetRadioBroadcast(string messageName)
    {
        var fleet = SpaceTraderConfig.FleetManager.GetFleetOf(PlayerShip.LocalPlayer.Ship);
        foreach (var member in fleet.Members)
        {
            Send(messageName, member);
       }
    }

    public void SendTargetRadioBroadcast(string messageName)
    {
        var target = PlayerShip.LocalPlayer.Ship.Target;
        var targetShip = target.GetComponent<Ship>();
        if (targetShip)
        {
            Send(messageName, targetShip);
        }
        else
        {
            Cancel();
        }
    }

    public void OnScreenActive()
    {
        Cancel();
    }
}


