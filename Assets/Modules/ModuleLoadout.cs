using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class ModuleLoadout
{
    [SerializeField]
    private List<ModuleStatus> hardpointModules;

    public List<ModuleStatus> HardpointModules
    {
        get { return hardpointModules; }
    }
    
	public void Activate(Ship ship, int slot)
	{
        var module = HardpointModules[slot];

        if (module.ModuleType)
        {
            module.Activate(ship, slot);
        }
	}

    public void Equip(int slot, ModuleItemType moduleType)
    {
        hardpointModules[slot] = ModuleStatus.Create(moduleType);
    }
        
	public void Update()
	{
		foreach (var module in hardpointModules)
		{
			if (module.ModuleType)
			{
				module.Update();
			}
		}
	}
}
