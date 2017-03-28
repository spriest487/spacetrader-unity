#pragma warning disable 0649

using UnityEngine;
using System.Collections.Generic;

public class Fleet : ScriptableObject
{
    public const int MaxSize = 5;

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

    // max size, from the leader's skills
    public int Capacity
    {
        get
        {
            if (!leader)
            {
                return 0;
            }

            var captain = leader.GetCaptain();
            if (!captain)
            {
                return 1;
            }

            return 1 + captain.PilotSkill; //todo: leader skill?
        }
    }

    public Fleet()
    {
        followers = new List<Ship>(MaxSize);
    }

    private void OnDestroy()
    {
        leader = null;
        followers.Clear();
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

    public Vector3 GetFormationPos(Ship ship)
    {
        var positionIndex = Followers.IndexOf(ship);
        Debug.Assert(positionIndex >= 0, "can't use GetFormationPos for ships that aren't fleet members");

        /* there can only be 4 member so this doesn't need to be too complicated */
        var offset = Vector3.right * Leader.Collider.bounds.extents.magnitude * (positionIndex / 2 + 1);
        
        if (positionIndex % 2 != 0)
        {
            offset = -offset;
        }

        return Leader.transform.localToWorldMatrix.MultiplyPoint(offset);
    }
}
