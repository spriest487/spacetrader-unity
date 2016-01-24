using UnityEngine;
using System;

public class Market : ScriptableObject {
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Assets/Create/SpaceTrader/Market")]
    public static void CreateNewMarket()
    {
        ScriptableObjectUtility.CreateAsset<Market>();
    }
#endif

    [SerializeField]
    private int baseHirePrice;

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
}
