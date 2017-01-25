#pragma warning disable 0649

using UnityEngine;
using System.Collections.Generic;

public class FleetScreen : MonoBehaviour
{
    [SerializeField]
    private FleetShipItem fleetShipItem;

    [SerializeField]
    private Transform shipListRoot;

    [SerializeField]
    private GUIElement availableShipsPanel;

    [SerializeField]
    private CanvasGroup fleetListGroup;

    private PooledList<FleetShipItem, Ship> fleetShips;

    private void OnEnable()
    {
        SpaceTraderConfig.OnLocalPlayerChanged += Refresh;

        Refresh();

        if (Application.isEditor)
        {
            Invoke("Refresh", 0.1f);
        }

        availableShipsPanel.gameObject.SetActive(false);
        fleetListGroup.interactable = true;
    }

    private void OnDisable()
    {
        SpaceTraderConfig.OnLocalPlayerChanged -= Refresh;
    }

    private void Awake()
    {
        availableShipsPanel.OnTransitionedOut += () => fleetListGroup.interactable = true;
    }

    private void ShowAvailableShipsPanel()
    {
        availableShipsPanel.Activate(true);
        fleetListGroup.interactable = false;
    }

    private void Refresh()
    {
        if (fleetShips == null)
        {
            fleetShips = new PooledList<FleetShipItem, Ship>(shipListRoot, fleetShipItem);
        }

        var player = SpaceTraderConfig.LocalPlayer;
        if (!player || !player.Ship)
        {
            fleetShips.Clear();
            return;
        }

        var shipsInFleet = new List<Ship>();

        var fleet = SpaceTraderConfig.FleetManager.GetFleetOf(player.Ship);
        if (fleet)
        {
            shipsInFleet.AddRange(fleet.Members);
        }
        else
        {
            shipsInFleet.Add(player.Ship);
        }

        var dockedAtStation = player.Moorable.DockedAtStation;
        if (dockedAtStation)
        {
            //blank item to display the buy options
            shipsInFleet.Add(null);
        }

        fleetShips.Refresh(shipsInFleet, (i, item, ship) => item.Assign(ship, dockedAtStation));
    }
}