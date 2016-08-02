using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

[Serializable]
public class ModuleLoadout : IEnumerable<HardpointModule>
{
    [SerializeField]
    private List<HardpointModule> hardpointModules;

    public ModuleLoadout()
    {
        hardpointModules = new List<HardpointModule>();
    }

    public IEnumerator<HardpointModule> GetEnumerator()
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
            int addedSlots = hardpointModules.Resize(value);

            //fill in any new slots with empty moduleslots
            for (int i = 0; i < addedSlots; ++i)
            {
                int addedSlot = (value - addedSlots) + i;
                Equip(addedSlot, null);
            }
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
        Debug.Assert(IsValidSlot(slot), "slots passed to ModuleLoadout.Equip() must be valid slot numbers");

        var free = IsFreeSlot(slot);
        if (!free)
        {
            if (moduleType)
            {
                Debug.LogWarning("equipping a module to an occupied slot!");
            }
        }

        hardpointModules[slot] = new HardpointModule(moduleType);
    }

    public void Swap(int slot1, int slot2)
    {
        Debug.Assert(IsValidSlot(slot1) && IsValidSlot(slot2),
            "slots passed to ModuleLoadout.Swap() must be valid slot numbers");

        var swapped = hardpointModules[slot1];
        hardpointModules[slot1] = new HardpointModule(hardpointModules[slot2].ModuleType);
        hardpointModules[slot2] = new HardpointModule(swapped.ModuleType);
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

    public HardpointModule GetSlot(int slot)
    {
        return hardpointModules[slot];
    }

    public bool IsFreeSlot(int slot)
    {
        return hardpointModules[slot] == null;
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
}
