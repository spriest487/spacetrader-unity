using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class FleetManager : ScriptableObject, ISerializationCallbackReceiver
{
    private class LeaderComparer : IEqualityComparer<Fleet>
    {
        public bool Equals(Fleet f1, Fleet f2)
        {
            return f1.Leader == f2.Leader;
        }

        public int GetHashCode(Fleet f)
        {
            return f.Leader.GetInstanceID();
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
        if (fleets != null)
        {
            fleetsList = new List<Fleet>(fleets.Values.Distinct(new LeaderComparer()));
        }
        else
        {
            fleetsList = null;
        }        
    }
    
    public void OnAfterDeserialize()
    {
        fleets = new Dictionary<Ship, Fleet>(fleetsList.Count);
        fleetsList.ForEach(fleet =>
        {
            if (!fleet.Leader)
            {
                Debug.LogWarningFormat("bad fleet when deserializing (no leader, {0} members)", fleet.Followers.Count());
                return;
            }

            fleets.Add(fleet.Leader, fleet);
            fleet.Followers.ForEach(follower => fleets.Add(follower, fleet));
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

                if (fleet.Followers.Count == 0)
                {
                    DisbandFleet(fleet);
                }
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
