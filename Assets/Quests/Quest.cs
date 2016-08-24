using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public abstract class Quest : ScriptableObject
{
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
    private SpaceStation station;
    
    public SpaceStation Station { get { return station; } }

    public static Quest Create(Quest template, SpaceStation station)
    {
        var quest = Instantiate(template);
        quest.name = template.name;
        quest.station = station;

        return quest;
    }

    public virtual void NotifyDeath(Ship ship, Ship killer)
    {
    }
}