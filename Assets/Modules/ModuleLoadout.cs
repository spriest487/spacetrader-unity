using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class ModuleLoadout
{
    [SerializeField]
    private List<ModuleStatus> hardpointModules;

    public IList<ModuleStatus> HardpointModules
    {
        get { return hardpointModules; }
    }

    public ModuleLoadout()
    {
        hardpointModules = new List<ModuleStatus>();
    }

    public void Activate(Ship ship, int slot)
    {
        var module = hardpointModules[slot];

        if (module.ModuleType)
        {
            module.Activate(ship, slot);
        }
    }

    public void Equip(int slot, ModuleItemType moduleType)
    {
        if (!IsValidSlot(slot) || !IsFreeSlot(slot))
        {
            throw new ArgumentException("invalid slot passed to Equip()");
        }

        hardpointModules[slot] = ModuleStatus.Create(moduleType);
    }

    public void RemoveAt(int slot)
    {
        if (IsFreeSlot(slot) || !IsValidSlot(slot))
        {
            throw new ArgumentException("invalid slot passed to RemoveAt()");
        }

        hardpointModules[slot] = null;
    }

    public bool IsFreeSlot(int slot)
    {
        return !hardpointModules[slot];
    }

    public int FindFirstFreeSlot()
    {
        for (int slot = 0; slot < hardpointModules.Count; ++slot)
        {
            if (!hardpointModules[slot])
            {
                return slot;
            }
        }

        return -1;
    }

    public bool IsValidSlot(int slot)
    {
        return slot >= 0 && slot < hardpointModules.Count;
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
