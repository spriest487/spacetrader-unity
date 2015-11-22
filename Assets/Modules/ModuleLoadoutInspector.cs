using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ModuleLoadout))]
public class ModuleLoadoutInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        //force init of FrontModules
        var modules = (target as ModuleLoadout).FrontModules;
    }
}