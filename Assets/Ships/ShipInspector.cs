using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Ship))]
public class ShipInspector : Editor
{
    private ModulePreset applyPreset;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var ship = target as Ship;

        if (GUILayout.Button("Reset cargo hold"))
        {
            ship.Cargo = CreateInstance<CargoHold>();
        }

        if (GUILayout.Button("Reset weapon hardpoints"))
        {
            for (int mod = 0; mod < ship.ModuleLoadout.HardpointModules.Count; ++mod)
            {
                ship.ModuleLoadout.HardpointModules[mod] = ModuleStatus.Create(null);
            }
        }
        
        applyPreset = EditorGUILayout.ObjectField("Select module preset", applyPreset, typeof(ModulePreset), false) as ModulePreset;
        GUI.enabled = applyPreset;
        if (GUILayout.Button("Apply"))
        {
            applyPreset.Apply(ship);
            applyPreset = null;
        }
        GUI.enabled = true;
    }
}