#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class FleetList : MonoBehaviour
{
    private BracketManager bracketManager;

    private List<Ship> currentShips;

    [SerializeField]
    private List<FleetListItem> items;

    [SerializeField]
    private FleetListItem itemPrefab;

    [SerializeField]
    private Transform content;

    private void Start()
    {
        var hud = GetComponentInParent<HUD>();
        bracketManager = hud.GetComponentInChildren<BracketManager>();
    }

    private void Clear()
    {
        items.ForEach(item => Destroy(item.gameObject));
        items.Clear();

        currentShips = null;
    }

    public void Update()
    {
        var player = PlayerShip.LocalPlayer;
        if (!player || !player.Ship)
        {
            Clear();
            return;
        }

        var fleet = SpaceTraderConfig.FleetManager.GetFleetOf(player.Ship);
        if (!fleet)
        {
            Clear();
            return;
        }
        
        var ships = new List<Ship>(fleet.Members);
        ships.RemoveAll(ship => ship == player.Ship);

        bool dirty = currentShips == null || !currentShips.ElementsEquals(ships) || currentShips.Any(s => !s);

        if (dirty)
        {
            Clear();
            currentShips = ships;

            ships.ForEach(ship =>
            {
                var newItem = FleetListItem.CreateFromPrefab(itemPrefab, ship, bracketManager.FleetMemberColor);
                newItem.transform.SetParent(content, false);

                items.Add(newItem);
            });
        }
    }
}
