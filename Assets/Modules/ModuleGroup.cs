using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class ModuleGroup : ScriptableObject, IEnumerable<ModuleStatus>
{
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
    internal void PopulateSlots()
    {
        var existingIds = new HashSet<int>();

        for (int i = 0; i < modules.Length; ++i)
        {
            //remove duplicates
            if (modules[i] != null && existingIds.Contains(modules[i].GetInstanceID()))
            {
                modules[i] = null;
            }

            //fill in blanks
            if ((modules[i]) == null)
            {
                modules[i] = ModuleStatus.Create(null);
            }

            existingIds.Add(modules[i].GetInstanceID());
        }
    }
#endif
    
	public void Resize(int size)
	{
		if (size < 0)
		{
			throw new ArgumentException("size must be 0 or greater");
		}

        int currentSize = modules != null ? modules.Length : 0;

		ModuleStatus[] newModules = new ModuleStatus[size];
		for (var slot = 0; slot < size; ++slot)
		{
			if (slot < currentSize)
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
	
	public void Equip(int slot, ModuleDefinition definition)
	{
		if (slot < 0 || slot >= modules.Length)
		{
			throw new ArgumentException("not a valid slot: " + slot);
		}

		if (definition != null)
		{
			modules[slot] = ModuleStatus.Create(definition);
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

#if UNITY_EDITOR

[UnityEditor.CustomEditor(typeof(ModuleGroup))]
public class ModuleGroupInspector : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        var group = (ModuleGroup)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Populate slots"))
        {
            group.PopulateSlots();
        }
    }
}

#endif