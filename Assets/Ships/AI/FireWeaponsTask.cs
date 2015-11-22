using System;
using UnityEngine;

class FireWeaponsTask : AITask
{
    [SerializeField]
    private ModuleLoadout loadout;

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

    public override void Begin()
    {
        loadout = TaskFollower.GetComponent<ModuleLoadout>();
    }

    public override void Update()
    {
        var ship = TaskFollower.Captain.Ship;

        if (!loadout || !ship.Target)
        {
            return;
        }

        for (int module = 0; module < loadout.FrontModules.Size; ++module)
        {
            loadout.FrontModules[module].Aim = ship.Target.transform.position;

            loadout.Activate(module);
        }
    }
}