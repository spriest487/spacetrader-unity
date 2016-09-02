﻿using UnityEngine;
using System.Collections;
using System;

public class WorldMapArea : ActionOnActivate
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
        return true;
    }
}