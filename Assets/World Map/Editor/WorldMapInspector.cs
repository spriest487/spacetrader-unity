using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WorldMapArea))]
public class WorldMapInspector : Editor
{ 
    public override void OnInspectorGUI()
    {
        var worldMapArea = target as WorldMapArea;
        DrawDefaultInspector();

        GUILayout.Label("Editor", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Layout now"))
        {
            worldMapArea.ForceUpdateLayout();
        }
    }
}
