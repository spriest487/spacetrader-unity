using UnityEngine;
using System;

public abstract class Quest : ScriptableObject
{
    [Serializable]
    public struct LocationID
    {
        public string Area;
        public string Station;
    }

    public abstract int XPReward { get; }
    public abstract int MoneyReward { get; }
    
    public abstract void OnFinish(Quest quest);
    public abstract void OnAbandon(Quest quest);

    public abstract bool Done { get; }
    public abstract string Description { get; }

    public PlayerShip Owner
    {
        get
        {
            //TODO: cache?
            return SpaceTraderConfig.QuestBoard.OwnerOf(this);
        }
    }

    [SerializeField]
    private LocationID location;
    
    public LocationID Location { get { return location; } }

    public static Quest Create(Quest template, SpaceStation station)
    {
        var quest = Instantiate(template);
        quest.name = template.name;
        quest.location = new LocationID
        {
            Area = SpaceTraderConfig.WorldMap.GetCurrentArea().name,
            Station = station.name
        };

        return quest;
    }

    public virtual void NotifyDeath(Ship ship, Ship killer)
    {
    }
}