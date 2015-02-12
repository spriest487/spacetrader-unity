#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(ModuleLoadout))]
public class ModuleLoadoutInspector : Editor
{
	public override void OnInspectorGUI()
	{
        base.OnInspectorGUI();

		var loadout = (ModuleLoadout) target;

        //TODO!
        string[] moduleNames = { "Laser Gun", "Heavy Laser Gun" };
        		
		var moduleGroups = new []{ loadout.FrontModules };
		foreach (var moduleGroup in moduleGroups)
		{
            if (moduleGroup.foldoutState = EditorGUILayout.Foldout(moduleGroup.foldoutState, "Front modules"))
			{
				EditorGUI.indentLevel = 1;	

				var moduleCount = EditorGUILayout.IntField("Module count", moduleGroup.Size);
				if (moduleCount != moduleGroup.Size)
				{
					moduleGroup.Resize(moduleCount);
				}

				if (loadout.FrontModules.Size == 0)
				{
					EditorGUILayout.LabelField("No modules equipped");
				}
				
				for (int slot = 0; slot < moduleGroup.Size; ++slot)
				{
					if (!moduleGroup[slot].Empty)
					{
						EditorGUI.indentLevel = 2;

						var module = moduleGroup[slot];

                        if (module.FoldoutState = EditorGUILayout.Foldout(module.FoldoutState, module.Name))
                        {
                            var cooldownText = module.Cooldown > 0 ? module.Cooldown.ToString("F2") : "(ready)";

                            EditorGUILayout.LabelField("Cooldown", cooldownText);

                            if (GUILayout.Button("Clear"))
                            {
                                moduleGroup.Equip(slot, null);
                            }
                        }

						EditorGUI.indentLevel = 1;
					}
					else
					{
                        var selectableModules = new string[moduleNames.Length + 1];
						selectableModules[0] = "Select module";
                        for (int name = 0; name < moduleNames.Length; ++name)
						{
                            selectableModules[name + 1] = moduleNames[name];
						}

						var selectedName = EditorGUILayout.Popup("Module type", 0, selectableModules);
						if (selectedName != 0)
						{
							moduleGroup.Equip(slot, selectableModules[selectedName]);
						}
					}
				}

				EditorGUI.indentLevel = 0;
			}
		}			
	}
}

#endif