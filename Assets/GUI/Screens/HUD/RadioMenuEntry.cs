#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class RadioMenuEntry : MonoBehaviour
{
    [SerializeField]
    private bool requiresTarget;

    public bool RequiresTarget { get { return requiresTarget; } }

    public void RefreshRadioMenu()
    {
        var player = PlayerShip.LocalPlayer;
        var button = GetComponent<Button>();

        button.interactable = !requiresTarget || (player && player.Ship.Target);
    }
}