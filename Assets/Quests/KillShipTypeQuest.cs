using System;
using UnityEngine;

public class KillShipTypeQuest : Quest
{
    [Header("Config")]

    [SerializeField]
    private int targetCount;

    [SerializeField]
    private string shipTypeName;

    [Header("Runtime")]

    [SerializeField]
    private int killCount;

    public override string Description
    {
        get
        {
            return string.Format("Kill {0} {1}s", targetCount, shipTypeName);
        }
    }

    public override bool Done
    {
        get
        {
            return killCount >= targetCount;
        }
    }

    private ShipType GetShipType()
    {
        return SpaceTraderConfig.Market.BuyableShipTypes.Find(s => s.name == shipTypeName);
    }

    public override void NotifyDeath(Ship ship, Ship killer)
    {
        if (Done)
        {
            return;
        }
        
        if (killer == Owner.Ship && ship.ShipType == GetShipType())
        {
            ++killCount;
        }
    }

    public override int MoneyReward
    {
        get
        {
            return targetCount * SpaceTraderConfig.Market.GetShipPrice(GetShipType()) / 10;
        }
    }

    public override int XPReward
    {
        get
        {
            return targetCount * GetShipType().XPReward / 2;
        }
    }

    public override void OnFinish(Quest quest)
    {
    }

    public override void OnAbandon(Quest quest)
    {
    }
}