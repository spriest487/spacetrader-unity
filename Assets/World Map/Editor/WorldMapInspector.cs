using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WorldMap))]
public class WorldMapInspector : Editor
{ 
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var worldMap = target as WorldMap;

        GUILayout.Label("Editor", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Layout now"))
        {
            foreach (var area in worldMap.GetComponentsInChildren<WorldMapArea>())
            {
                area.ForceUpdateLayout();
            }
        }
    }
}
