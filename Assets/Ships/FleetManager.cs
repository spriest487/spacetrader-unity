using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class FleetManager : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField]
    private List<Fleet> fleetsList;

    private Dictionary<Ship, Fleet> fleets;

    public FleetManager()
    {
        fleets = new Dictionary<Ship, Fleet>();
    }

    public void OnBeforeSerialize()
    {
        fleetsList = new List<Fleet>(fleets.Values.Distinct());
    }
    
    public void OnAfterDeserialize()
    {
        fleets = new Dictionary<Ship, Fleet>(fleetsList.Count);
        fleetsList.ForEach(fleet =>
        {
            if (fleet)
            {
                fleets.Add(fleet.Leader, fleet);
                fleet.Followers.ForEach(follower => fleets.Add(follower, fleet));
            }
        });
        fleetsList = null;
    }

    public void AddToFleet(Ship leader, Ship follower)
    {
        if (fleets.ContainsKey(follower))
        {
            LeaveFleet(follower);
        }

        Fleet fleet;
        if (!fleets.TryGetValue(leader, out fleet))
        {
            fleet = Fleet.Create(leader);
            fleets.Add(leader, fleet);
        }

        fleet.Followers.Add(follower);        
        fleets.Add(follower, fleet);
    }

    public void LeaveFleet(Ship ship)
    {
        Fleet fleet;
        if (fleets.TryGetValue(ship, out fleet))
        {
            if (ship == fleet.Leader)
            {
                DisbandFleet(fleet);
            }
            else
            {
                fleet.Followers.Remove(ship);
            }

            fleets.Remove(ship);
        }
    }

    private void DisbandFleet(Fleet fleet)
    {
        fleet.Followers.ForEach(ship => fleets.Remove(ship));
        fleets.Remove(fleet.Leader);

        Destroy(fleet);
    }

    public Fleet GetFleetOf(Ship ship)
    {
        Fleet fleet;
        return fleets.TryGetValue(ship, out fleet) ? fleet : null;
    }
}
