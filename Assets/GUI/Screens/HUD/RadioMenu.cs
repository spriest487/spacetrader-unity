#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(GUIElement))]
public class RadioMenu : MonoBehaviour
{
    [SerializeField]
    private Button greetingButton;
    private Text greetingText;

    private void Awake()
    {
        greetingText = greetingButton.GetComponentInChildren<Text>();
    }

    private void OnEnable()
    {
        foreach (var entry in GetComponentsInChildren<RadioMenuEntry>())
        {
            entry.RefreshRadioMenu();
        }
    }

    private void Update()
    {
        var player = SpaceTraderConfig.LocalPlayer;

        if (player.Ship.Target)
        {
            const string format = "Greetings, {0}!";
            greetingText.text = string.Format(format, player.Ship.Target.name);
            greetingButton.interactable = true;
        }
        else
        {
            greetingText.text = "Greetings!";
            greetingButton.interactable = false;
        }
    }

    private void Send(string messageName, Ship target)
    {
        var message = (RadioMessageType)Enum.Parse(typeof(RadioMessageType), messageName);
        var source = PlayerShip.LocalPlayer.Ship;

        source.SendRadioMessage(message, target);
        GetComponent<GUIElement>().Dismiss();
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
            GetComponent<GUIElement>().Dismiss();
        }
    }
}


