using UnityEngine;
using System.Collections;
using System;

public partial class WorldMapArea : ActionOnActivate
{
    public override string ActionName
    {
        get
        {
            return "Jump to " + name;
        }
    }

    public override void Activate(Ship activator)
    {
        activator.JumpTo(this);
    }

    public override bool CanBeActivatedBy(Ship activator)
    {
        return SpaceTraderConfig.WorldMap.GetCurrentArea() != this;
    }
}
