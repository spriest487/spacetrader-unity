using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

[Serializable]
public class ModuleLoadout : IEnumerable<ModuleStatus>
{
    [SerializeField]
    private List<ModuleStatus> hardpointModules;
    
    public ModuleLoadout()
    {
        hardpointModules = new List<ModuleStatus>();
    }
    
    public IEnumerator<ModuleStatus> GetEnumerator()
    {
        return hardpointModules.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int SlotCount
    {
        get { return hardpointModules.Count; }
        set
        {
            hardpointModules.Resize(value);
        }
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
        if (!IsValidSlot(slot))
        {
            Debug.LogError("invalid slot passed to ModuleLoadout.Equip()");
            return;
        }

        var free = IsFreeSlot(slot);
        if (!free)
        {
            if (moduleType)
            {
                Debug.LogWarning("equipping a module to an occupied slot!");
            }
        }

        hardpointModules[slot] = new ModuleStatus(moduleType);
    }

    public ModuleItemType RemoveAt(int slot)
    {
        if (IsFreeSlot(slot) || !IsValidSlot(slot))
        {
            Debug.LogError("invalid slot passed to ModuleLoadout.RemoveAt()");
            return null;
        }

        var result = hardpointModules[slot].ModuleType;
        Equip(slot, null);

        return result;
    }

    public ModuleStatus GetSlot(int slot)
    {
        return hardpointModules[slot];
    }

    public bool IsFreeSlot(int slot)
    {
        return !hardpointModules[slot].ModuleType;
    }

    public int FindFirstFreeSlot()
    {
        for (int slot = 0; slot < hardpointModules.Count; ++slot)
        {
            if (!hardpointModules[slot].ModuleType)
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
