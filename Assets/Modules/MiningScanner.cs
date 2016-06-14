
using System;
using UnityEngine;

public class MiningScanner : ModuleBehaviour
{
    public override string Description
    {
        get
        {
            return "Scans asteroids for mineral deposits";
        }
    }

    public override void Activate(Ship activator, int slot)
    {
        var target = activator.Target;
        if (!target)
        {
            return;
        }

        var asteroid = target.GetComponent<Asteroid>();
        if (!asteroid)
        {
            return;
        }
    }
}