using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(ShipSpawner))]
public class ShipSpawnerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("New Captain"))
        {
            var prop = serializedObject.FindProperty("captain");
            prop.objectReferenceValue = CrewMember.CreateRandom();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
