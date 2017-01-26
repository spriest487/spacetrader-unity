#pragma warning disable 0649

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using MarketRequests;

[CreateAssetMenu(menuName = "SpaceTrader/Market")]
public class Market : ScriptableObject
{
    public static string FormatCurrency(int amount)
    {
        return "*" + amount.ToString("N0");
    }

    [SerializeField]
    private int baseHirePrice;

    [SerializeField]
    private List<ShipForSale> shipPrices;

    public List<ShipType> BuyableShipTypes
    {
        get
        {
            var types = new List<ShipType>();
            foreach (var ship in shipPrices)
            {
                types.Add(ship.ShipType);
            }
            return types;
        }
    }

    public int GetHirePrice(CrewMember crewMember)
    {
        return baseHirePrice * crewMember.Level;
    }

    public void HireCrewMember(PlayerShip player, SpaceStation fromStation, CrewMember crewMember)
    {
        Debug.Assert(fromStation == crewMember.AtStation, "can't buy crew who aren't available to buy");

        var ship = player.Ship;
        Debug.Assert(ship != null, "can't buy stuff without a ship");

        //check ship has space
        var passengerCount = ship.GetPassengers().Count();
        if (passengerCount == ship.CurrentStats.PassengerCapacity)
        {
            throw new InvalidOperationException("no room for more passengers");
        }

        //check player has enough money
        var price = GetHirePrice(crewMember);
        if (player.Money < price)
        {
            throw new InvalidOperationException("player can't afford price of " +price);
        }

        player.AddMoney(-price);
        crewMember.Assign(ship, CrewAssignment.Passenger);
    }

    public void FireCrewMember(PlayerShip player, SpaceStation atStation, CrewMember crewMember)
    {
        var ship = player.Ship;
        Debug.Assert(ship != null, "can't sell stuff without a ship");
        Debug.Assert(crewMember == ship.GetCaptain() || ship.GetPassengers().Contains(crewMember), "can't fire someone who doesn't work for you");

        crewMember.Unassign(atStation);
    }

    private ShipForSale GetShipForSale(ShipType type)
    {
        foreach (var ship in shipPrices)
        {
            if (ship.ShipType == type)
            {
                return ship;
            }
        }

        throw new InvalidOperationException("ship type not in list of buyable types");
    }

    public int GetShipPrice(ShipType type)
    {
        return GetShipForSale(type).Price;
    }

    public int GetShipSellingPrice(ShipType type)
    {
        return GetShipForSale(type).Price;
    }

    public void BuyShip(PlayerShip player, ShipType shipType, SpaceStation atStation)
    {
        var shipForSale = GetShipForSale(shipType);
        var oldShip = player.Ship;

        //check price
        Debug.Assert(player.Money >= shipForSale.Price,
            "player can't afford to buy ship");

        var passengers = oldShip.GetPassengers();
        var captain = oldShip.GetCaptain();

        //check crew space
        Debug.Assert(shipForSale.ShipType.Stats.PassengerCapacity < passengers.Count(),
            "ship being bought doesn't have enough room for existing passengers");

        var newShip = shipType.CreateShip(player.transform.position, player.transform.rotation);

        //copy player
        var newPlayer = newShip.gameObject.AddComponent<PlayerShip>();
        newPlayer.AddMoney(player.Money - shipForSale.Price);

        //copy crew
        captain.Assign(newShip, CrewAssignment.Captain);
        foreach (var passenger in passengers)
        {
            passenger.Assign(newShip, CrewAssignment.Passenger);
        }

        Destroy(player.gameObject);
        Universe.LocalPlayer = newPlayer;
    }

    public int GetSellingItemPrice(ItemType itemType, SpaceStation atStation)
    {
        return itemType.BaseValue;
    }

    public int GetBuyingItemPrice(ItemType itemType, SpaceStation atStation)
    {
        return itemType.BaseValue;
    }

    public void BuyItemFromStation(PlayerShip player, int itemIndex)
    {
        var station = player.Dockable.DockedAtStation;
        var playerCargo = player.Ship.Cargo;
        var stationCargo = station.ItemsForSale;

        var itemType = stationCargo[itemIndex];

        var price = GetBuyingItemPrice(itemType, station);

        Debug.Assert(itemIndex < stationCargo.Size,
            "index of item to buy must be valid index of item in station's cargo");
        Debug.Assert(playerCargo.FreeCapacity > 0,
            "player must have space to put bought item");
        Debug.Assert(player.Money >= price, "player must have enough money to buy item");

        stationCargo.RemoveAt(itemIndex);
        playerCargo.Add(itemType);

        player.AddMoney(-price);
    }

    public void SellItemToStation(PlayerShip player, int itemIndex)
    {
        var playerCargo = player.Ship.Cargo;
        var station = player.Dockable.DockedAtStation;
        var stationCargo = station.ItemsForSale;

        var itemType = playerCargo[itemIndex];

        var price = GetSellingItemPrice(itemType, station);

        Debug.Assert(playerCargo && itemIndex < playerCargo.Size,
            "index of item to sell must be valid index of item in player's cargo");

        if (stationCargo.FreeCapacity == 0)
        {
            //station cargo space is infinite
            ++stationCargo.Size;
        }

        playerCargo.RemoveAt(itemIndex);
        stationCargo.Add(itemType);

        player.AddMoney(price);
    }

    public bool TrySellShipToStation(PlayerShip player, Ship soldShip, out string error)
    {
        if (!player || !player.Ship || !soldShip)
        {
            error = "Can't sell ship due to an error";
            return false;
        }

        var station = player.Moorable.DockedAtStation;
        if (!station)
        {
            error = "Can only sell ships when docked";
            return false;
        }

        var fleet = Universe.FleetManager.GetFleetOf(player.Ship);
        var sellableShips = fleet.Members.Where(m => m != player.Ship)
            .ToList();

        if (!sellableShips.Contains(soldShip))
        {
            error = "Can't sell that ship";
            return false;
        }

        //convert crew into unassigned characters
        foreach (var spareCrew in soldShip.GetAllCrew())
        {
            spareCrew.Assign(player.Ship, CrewAssignment.Passenger);
        }

        int totalValue = GetShipSellingPrice(soldShip.ShipType);

        //sell all items
        for (int slot = 0; slot < soldShip.ModuleLoadout.SlotCount; ++slot)
        {
            var item = soldShip.ModuleLoadout.RemoveAt(slot);
            if (item)
            {
                totalValue += GetSellingItemPrice(item, station);
            }
        }

        for (int slot = 0; slot < soldShip.Cargo.Size; ++slot)
        {
            var item = soldShip.Cargo.RemoveAt(slot);
            if (item)
            {
                totalValue += GetSellingItemPrice(item, station);
            }
        }

        Universe.FleetManager.LeaveFleet(soldShip);

        Destroy(soldShip.gameObject);
        player.AddMoney(totalValue);

        error = null;
        return true;
    }

    private IEnumerator TakeLoot(PlayerTakeLootRequest request)
    {
        var items = request.Loot.Ship.Cargo;
        var activator = request.Player.Ship;

        var freeSpace = activator.Cargo.FreeCapacity;

        const string notEnoughSpace = "Not enough free cargo space";

        if (request.ItemIndex < 0)
        {
            var itemCount = items.ItemCount;

            if (freeSpace >= itemCount)
            {
                for (int slot = 0; slot < items.Size; ++slot)
                {
                    if (items.IsIndexFree(slot))
                    {
                        continue;
                    }

                    var item = items[slot];
                    activator.Cargo.Add(item);
                    items.RemoveAt(slot);
                }

                request.Done = true;
            }
            else
            {
                request.Error = notEnoughSpace;
            }
        }
        else
        {
            if (freeSpace > 0)
            {
                if (!items.IsIndexFree(request.ItemIndex) && items.IsValidIndex(request.ItemIndex))
                {
                    var itemType = items[request.ItemIndex];

                    activator.Cargo.Add(itemType);
                    items.RemoveAt(request.ItemIndex);

                    request.Done = true;
                }
                else
                {
                    request.Error = "Item not found";
                }
            }
            else
            {
                request.Error = notEnoughSpace;
            }
        }

        if (items.FreeCapacity == items.Size)
        {
            Destroy(request.Loot.gameObject);
        }

        yield break;
    }

    public void PlayerTakeLoot(PlayerTakeLootRequest request)
    {
        Universe.Instance.StartCoroutine(TakeLoot(request));
    }
}
