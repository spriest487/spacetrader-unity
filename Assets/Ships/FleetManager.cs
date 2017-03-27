using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "SpaceTrader/Fleet Manager")]
public class FleetManager : ScriptableObject
{
    [SerializeField]
    private List<Fleet> fleets = new List<Fleet>();

    /// <summary>
    /// add "follower" to the fleet of "leader". works even if "leader"
    /// isn't actually the leader of their own fleet
    /// </summary>
    public Fleet AddToFleet(Ship leader, Ship follower)
    {
        var hasFleet = fleets.Where(f => f.IsMember(follower)).Any();
        if (hasFleet)
        {
            LeaveFleet(follower);
        }

        var fleet = fleets.Where(f => f.IsMember(leader)).FirstOrDefault();
        if (!fleet)
        {
            fleet = Fleet.Create(leader);
            fleets.Add(fleet);
        }

        Debug.AssertFormat(fleet.Members.Count() < Fleet.MaxSize,
            "fleet must have {0} or fewer members",
            Fleet.MaxSize);

        fleet.Followers.Add(follower);

        return fleet;
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
        foreach (var fleet in fleets)
        {
            if (fleet.IsMember(ship))
            {
                return fleet;
            }
        }

        return null;
    }
}
