#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class FleetList : MonoBehaviour
{
    [SerializeField]
    private FleetListItem itemPrefab;

    [SerializeField]
    private Transform content;

    private BracketManager bracketManager;
    private PooledList<FleetListItem, Ship> items;

    private void Start()
    {
        var hud = GetComponentInParent<HUD>();
        bracketManager = hud.GetComponentInChildren<BracketManager>();
    }
    
    public void Update()
    {
        var player = PlayerShip.LocalPlayer;

        if (items == null)
        {
            items = new PooledList<FleetListItem, Ship>(content, itemPrefab);
        }

        Fleet playerFleet;
        if (!player 
            || !player.Ship 
            || !(playerFleet = SpaceTraderConfig.FleetManager.GetFleetOf(player.Ship)))
        {
            items.Clear();
        }
        else
        {
            var ships = playerFleet.Members.ToList();
            ships.Remove(player.Ship);

            items.Refresh(ships, (i, item, ship) => item.Assign(ship, bracketManager.FleetMemberColor));
        }
    }
}
