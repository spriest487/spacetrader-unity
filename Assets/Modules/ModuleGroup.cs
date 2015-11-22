using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class ModuleGroup : ScriptableObject, IEnumerable<ModuleStatus>
{
    [SerializeField]
    private ModuleLoadout loadout;

    [SerializeField]
	private ModuleStatus[] modules;
    
	public int Size
	{
		get
		{
			return modules.Length;
		}
	}

	public ModuleStatus this[int moduleIndex]
	{
		get
		{
			return modules[moduleIndex];
		}
	}

#if UNITY_EDITOR
    public void PopulateSlots()
    {
        for (int i = 0; i < modules.Length; ++i)
        {
            if ((modules[i]) == null)
            {
                modules[i] = ModuleStatus.Create(null, this);
            }
        }
    }
#endif

	public ModuleGroup()
	{
		Resize(0);
	}

    public void SetParent(ModuleLoadout loadout)
    {
        if (this.loadout != null || loadout == null)
        {
            throw new UnityException("there is no currently associated loadout or the new loadout is null");
        }

        this.loadout = loadout;
    }

	public void Resize(int size)
	{
		if (size < 0)
		{
			throw new ArgumentException("size must be 0 or greater");
		}

		ModuleStatus[] newModules = new ModuleStatus[size];
		for (var slot = 0; slot < size; ++slot)
		{
			if (slot < modules.Length)
			{
				newModules[slot] = modules[slot];
			}
			else
			{
				newModules[slot] = null;
			}
		}

		modules = newModules;
	}
	
	public void Equip(int slot, string moduleType)
	{
		if (slot < 0 || slot >= modules.Length)
		{
			throw new ArgumentException("not a valid slot: " + slot);
		}

		if (moduleType != null)
		{
			modules[slot] = ModuleStatus.Create(moduleType, this);
		}
		else
		{
			modules[slot] = null;
		}

	}

	public IEnumerator<ModuleStatus> GetEnumerator()
	{
		return ((IEnumerable<ModuleStatus>)modules).GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return modules.GetEnumerator();
	}
}