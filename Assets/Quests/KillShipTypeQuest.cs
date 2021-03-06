﻿#pragma warning disable 0649

using System;
using UnityEngine;

[CreateAssetMenu(menuName = "SpaceTrader/Quests/Generic Kill Quest")]
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

    public override QuestStatus Status
    {
        get
        {
            if (killCount >= targetCount)
            {
                return QuestStatus.Completed;
            }
            else
            {
                return base.Status;
            }
        }
    }

    private ShipType GetShipType()
    {
        return Universe.Market.BuyableShipTypes.Find(s => s.name == shipTypeName);
    }

    public override void NotifyDeath(Ship ship, Ship killer)
    {
        //already finished, or not even accepted yet? don't care
        if (killCount >= targetCount || !Owner)
        {
            return;
        }
        
        //did our owner score the kill? does it count as credit?
        if (killer == Owner.Ship && ship.ShipType == GetShipType())
        {
            ++killCount;
        }
    }

    public override int MoneyReward
    {
        get
        {
            return targetCount * Universe.Market.GetShipPrice(GetShipType()) / 10;
        }
    }

    public override int XPReward
    {
        get
        {
            return targetCount * GetShipType().XPReward / 2;
        }
    }
}