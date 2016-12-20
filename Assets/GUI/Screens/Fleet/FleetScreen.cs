using UnityEngine;
using System.Collections.Generic;

public class FleetScreen : MonoBehaviour
{
    [SerializeField]
    private FleetShipItem fleetShipItem;

    [SerializeField]
    private Transform shipListRoot;

    private PooledList<FleetShipItem, Ship> fleetShips;

    private void OnEnable()
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

        IEnumerable<Ship> shipsInFleet;
        var fleet = SpaceTraderConfig.FleetManager.GetFleetOf(player.Ship);
        if (fleet)
        {
            shipsInFleet = fleet.Members;
        }
        else
        {
            shipsInFleet = new [] { player.Ship };
        }

        fleetShips.Refresh(shipsInFleet, (i, item, ship) => { });
    }
}