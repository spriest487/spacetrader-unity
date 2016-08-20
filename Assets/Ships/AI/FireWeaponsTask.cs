using System;
using UnityEngine;

class FireWeaponsTask : AITask
{
    public override bool Done
    {
        get
        {
            //this task just fires once
            return true;
        }
    }

    public static FireWeaponsTask Create()
    {
        var task = CreateInstance<FireWeaponsTask>();
        return task;
    }
    
    public override void Update()
    {
        var ship = TaskFollower.Ship;

        if (!ship.Target)
        {
            return;
        }

        var loadout = ship.ModuleLoadout;
        for (int module = 0; module < loadout.SlotCount; ++module)
        {
            loadout.GetSlot(module).Aim = ship.Target.transform.position;

            loadout.Activate(ship, module);
        }
    }
}