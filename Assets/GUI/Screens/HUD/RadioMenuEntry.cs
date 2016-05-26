#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;

public enum RadioMenuEntryTarget
{
    Target,
    Fleet,
    Global
}

[RequireComponent(typeof(Button))]
public class RadioMenuEntry : MonoBehaviour
{
    [SerializeField]
    private RadioMenuEntryTarget target;

    public RadioMenuEntryTarget Target { get { return target; } }

    public void RefreshRadioMenu()
    {
        var player = PlayerShip.LocalPlayer;
        var button = GetComponent<Button>();

        switch (target)
        {
            case RadioMenuEntryTarget.Fleet:
                var fleet = SpaceTraderConfig.FleetManager.GetFleetOf(player.Ship);
                button.interactable = fleet;
                break;
            case RadioMenuEntryTarget.Target:
                button.interactable = player.Ship.Target;
                break;
            default:
                button.interactable = true;
                break;
        }
    }
}