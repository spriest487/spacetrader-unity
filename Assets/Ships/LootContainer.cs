#pragma warning disable 0649

using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Ship))]
public class LootContainer : ActionOnActivate
{
    public const float LOOT_DISTANCE = 5;

    public Ship Ship { get; private set; }

    public override string ActionName
    {
        get { return "OPEN CONTAINER"; }
    }

    public override void Activate(Ship activator)
    {
        Debug.Assert(SpaceTraderConfig.LocalPlayer.Ship == activator);

        ScreenManager.Instance.BroadcastScreenMessage(ScreenID.None, "OnPlayerActivatedLoot", this);
    }

    public override bool CanBeActivatedBy(Ship activator)
    {
        var distSqr = (activator.transform.position - transform.position).sqrMagnitude;
        var lootDistSqr = LOOT_DISTANCE * LOOT_DISTANCE;

        return distSqr < lootDistSqr;
    }

    void Start()
    {
        Ship = GetComponent<Ship>();
    }
}
