using UnityEngine;
using System.Collections.Generic;
using System.Collections;

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
	public bool foldoutState = true;
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
			throw new System.ArgumentException("size must be 0 or greater");
		}

		ModuleStatus[] newModules = new ModuleStatus[size];
		for (var slot = 0; slot < size; ++slot)
		{
			if (modules != null && slot < modules.Length)
			{
				newModules[slot] = modules[slot];
			}
			else
			{
				newModules[slot] = ModuleStatus.EMPTY;
			}
		}

		modules = newModules;
	}
	
	public void Equip(int slot, string moduleType)
	{
		if (slot < 0 || slot >= modules.Length)
		{
			throw new System.ArgumentException("not a valid slot: " + slot);
		}

		if (moduleType != null)
		{
			modules[slot] = new ModuleStatus(moduleType, this);
		}
		else
		{
			modules[slot] = ModuleStatus.EMPTY;
		}

	}

	public IEnumerator<ModuleStatus> GetEnumerator()
	{
		return ((IEnumerable<ModuleStatus>)modules).GetEnumerator();
	}

	System.Collections.IEnumerator IEnumerable.GetEnumerator()
	{
		return modules.GetEnumerator();
	}
}