﻿using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "SpaceTrader/Fleet Manager")]
public class FleetManager : ScriptableObject
{
    [SerializeField]
    private List<Fleet> fleets = new List<Fleet>();

    public void AddToFleet(Ship leader, Ship follower)
    {
        var hasFleet = fleets.Where(f => f.IsMember(follower)).Any();
        if (hasFleet)
        {
            LeaveFleet(follower);
        }

        var fleet = fleets.Where(f => f.Leader == leader).FirstOrDefault();
        if (!fleet)
        {
            fleet = Fleet.Create(leader);
            fleets.Add(fleet);
        }

        fleet.Followers.Add(follower);
    }

    public void LeaveFleet(Ship ship)
    {
        var fleet = fleets.Where(f => f.IsMember(ship)).FirstOrDefault();
        if (fleet)
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
        }
    }

    private void DisbandFleet(Fleet fleet)
    {
        fleets.Remove(fleet);
        Destroy(fleet);
    }

    public Fleet GetFleetOf(Ship ship)
    {
        return fleets.Where(f => f.IsMember(ship)).FirstOrDefault();
    }
}
