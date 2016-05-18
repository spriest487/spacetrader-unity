using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class FleetManager : ScriptableObject, ISerializationCallbackReceiver
{
    private class Fleet : ScriptableObject
    {
        public Ship Leader;
        public List<Ship> Followers;

        public Fleet()
        {
            Leader = null;
            Followers = new List<Ship>();
        }
    }
    
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
            fleets.Add(fleet.Leader, fleet);
            fleet.Followers.ForEach(follower => fleets.Add(follower, fleet));
        });
        fleetsList = null;
    }

    public void AddToFleet(Ship leader, Ship follower)
    {
        Fleet fleet;
        if (!fleets.TryGetValue(leader, out fleet))
        {
            fleet = CreateInstance<Fleet>();
        }

        fleet.Followers.Add(follower);
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
}
