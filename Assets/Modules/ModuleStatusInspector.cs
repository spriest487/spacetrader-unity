using UnityEditor;

[CustomEditor(typeof(ModuleGroup))]
public class ModuleStatusInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        (target as ModuleGroup).PopulateSlots();
    }
}
