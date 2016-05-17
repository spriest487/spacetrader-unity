#pragma warning disable 0649

using UnityEngine;
using System;
using System.Collections.Generic;

public class Market : ScriptableObject {
    public static string FormatCurrency(int amount)
    {
        return "*" + amount.ToString("N0");
    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Assets/Create/SpaceTrader/Market")]
    public static void CreateNewMarket()
    {
        ScriptableObjectUtility.CreateAsset<Market>();
    }
#endif

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
        return baseHirePrice;
    }

    public void HireCrewMember(PlayerShip player, SpaceStation fromStation, CrewMember crewMember)
    {
        Debug.Assert(fromStation.AvailableCrew.Contains(crewMember), "can't buy crew who aren't available to buy");

        var ship = player.Ship;
        Debug.Assert(ship != null, "can't buy stuff without a ship");

        //check ship has space
        var passengerCount = ship.CrewAssignments.Passengers.Count;
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
        ship.CrewAssignments.Passengers.Add(crewMember);
        fromStation.AvailableCrew.Remove(crewMember);
    }

    public void FireCrewMember(PlayerShip player, SpaceStation atStation, CrewMember crewMember)
    {
        var ship = player.Ship;
        Debug.Assert(ship != null, "can't sell stuff without a ship");
        Debug.Assert(ship.CrewAssignments.Passengers.Contains(crewMember), "can't fire someone who doesn't work for you");

        atStation.AvailableCrew.Add(crewMember);
        ship.CrewAssignments.Passengers.Remove(crewMember);
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

    public void BuyShip(PlayerShip player, ShipType shipType, SpaceStation atStation)
    {
        var shipForSale = GetShipForSale(shipType);
        var oldShip = player.Ship;

        //check price
        Debug.Assert(player.Money >= shipForSale.Price, 
            "player can't afford to buy ship");                

        //check crew space
        Debug.Assert(shipForSale.ShipType.Stats.PassengerCapacity < oldShip.CrewAssignments.Passengers.Count,
            "ship being bought doesn't have enough room for existing passengers");

        var newShip = shipType.CreateShip(player.transform.position, player.transform.rotation);
        
        //copy player
        var newPlayer = newShip.gameObject.AddComponent<PlayerShip>();
        newPlayer.AddMoney(player.Money - shipForSale.Price);

        //copy crew
        newShip.CrewAssignments.Captain = oldShip.CrewAssignments.Captain;
        newShip.CrewAssignments.Passengers = oldShip.CrewAssignments.Passengers;
            
        Destroy(player.gameObject);
        SpaceTraderConfig.LocalPlayer = newPlayer;
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
        var station = player.CurrentStation;
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
        var station = player.CurrentStation;
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
}
