#pragma warning disable 0649

using UnityEngine;
using System.Collections.Generic;

public class Fleet : ScriptableObject
{
    [SerializeField]
    private Ship leader;

    [SerializeField]
    private List<Ship> followers;

    public Ship Leader
    {
        get { return leader; }
    }
    public List<Ship> Followers
    {
        get { return followers; }
    }

    public IEnumerable<Ship> Members
    {
        get
        {
            yield return leader;
            foreach (var follower in followers)
            {
                yield return follower;
            }
        }
    }

    public Fleet()
    {
        followers = new List<Ship>();
    }

    public static Fleet Create(Ship leader)
    {
        var fleet = CreateInstance<Fleet>();
        fleet.leader = leader;

        return fleet;
    }

    public bool IsMember(Ship ship)
    {
        return Followers.Contains(ship) || leader == ship;
    }
}
